<?php
error_reporting(E_ALL);
ini_set('display_errors', 1);

header('Content-Type: application/json');
header('Access-Control-Allow-Origin: *');
header('Access-Control-Allow-Methods: GET, POST, OPTIONS');
header('Access-Control-Allow-Headers: Content-Type');

if ($_SERVER['REQUEST_METHOD'] === 'OPTIONS') {
    exit(0);
}

// ===== DATABASE CONNECTION =====
$host = "localhost";
$dbname = "survey_db";
$user = "root";
$pass = "";


try {
    $pdo = new PDO(
        "mysql:host=$host;dbname=$dbname;charset=utf8mb4",
        $user,
        $pass,
        [
            PDO::ATTR_ERRMODE => PDO::ERRMODE_EXCEPTION,
            PDO::ATTR_DEFAULT_FETCH_MODE => PDO::FETCH_ASSOC,
        ]
    );

    date_default_timezone_set('Asia/Manila');
    $pdo->exec("SET time_zone = '+08:00'");

} catch (Exception $e) {
    die(json_encode([
        'success' => false,
        'error' => 'Database connection failed: ' . $e->getMessage()
    ]));
}

$action = $_GET['action'] ?? '';
$data = json_decode(file_get_contents('php://input'), true) ?? [];

function jsonOut($data)
{
    echo json_encode($data);
    exit;
}

/* ========== CATEGORY CRUD ========== */
if ($action === 'get_survey') {
    try {
        $cats = $pdo->query("SELECT * FROM categories ORDER BY id ASC")->fetchAll(PDO::FETCH_ASSOC);
        foreach ($cats as &$c) {
            $q = $pdo->prepare("SELECT * FROM questions WHERE category_id = ? ORDER BY id ASC");
            $q->execute([$c['id']]);
            $c['questions'] = $q->fetchAll(PDO::FETCH_ASSOC);
        }
        
        // Fetch average answer time
        $avgStmt = $pdo->query("SELECT AVG(answer_time) AS avg_time FROM responses");
        $avgRow = $avgStmt->fetch(PDO::FETCH_ASSOC);
        $average_time = ($avgRow && $avgRow['avg_time'] !== null)
            ? round($avgRow['avg_time'])
            : null;
        
        jsonOut([
            "success" => true,
            "categories" => $cats,
            "average_time" => $average_time
        ]);
    } catch (Exception $e) {
        jsonOut([
            "success" => false,
            "error" => $e->getMessage()
        ]);
    }
}

if ($action === 'add_category') {
    try {
        $title = trim($data['title'] ?? '');
        if ($title === '') {
            jsonOut(["success" => false, "error" => "No title provided"]);
        }
        $stmt = $pdo->prepare("INSERT INTO categories (name) VALUES (?)");
        $stmt->execute([$title]);
        jsonOut(["success" => true]);
    } catch (Exception $e) {
        jsonOut(["success" => false, "error" => $e->getMessage()]);
    }
}

if ($action === 'update_category') {
    try {
        $stmt = $pdo->prepare("UPDATE categories SET name = ? WHERE id = ?");
        $stmt->execute([$data['title'], $data['id']]);
        jsonOut(["success" => true]);
    } catch (Exception $e) {
        jsonOut(["success" => false, "error" => $e->getMessage()]);
    }
}

if ($action === 'delete_category') {
    try {
        $stmt = $pdo->prepare("DELETE FROM categories WHERE id = ?");
        $stmt->execute([$data['id']]);
        jsonOut(["success" => true]);
    } catch (Exception $e) {
        jsonOut(["success" => false, "error" => $e->getMessage()]);
    }
}

/* ========== QUESTION CRUD ========== */
if ($action === 'add_question') {
    try {
        $stmt = $pdo->prepare("INSERT INTO questions (category_id, question_text) VALUES (?, ?)");
        $stmt->execute([$data['category_id'], $data['text']]);
        jsonOut(["success" => true]);
    } catch (Exception $e) {
        jsonOut(["success" => false, "error" => $e->getMessage()]);
    }
}

if ($action === 'update_question') {
    try {
        $stmt = $pdo->prepare("UPDATE questions SET question_text = ? WHERE id = ?");
        $stmt->execute([$data['text'], $data['id']]);
        jsonOut(["success" => true]);
    } catch (Exception $e) {
        jsonOut(["success" => false, "error" => $e->getMessage()]);
    }
}

if ($action === 'delete_question') {
    try {
        $stmt = $pdo->prepare("DELETE FROM questions WHERE id = ?");
        $stmt->execute([$data['id']]);
        jsonOut(["success" => true]);
    } catch (Exception $e) {
        jsonOut(["success" => false, "error" => $e->getMessage()]);
    }
}

/* ========== RESPONSES FETCH ========== */
if ($action === 'get_responses') {
    try {
        $days = $_GET['days'] ?? null;
        $cond = $days ? "WHERE submitted_at >= NOW() - INTERVAL $days DAY" : "";
        $resp = $pdo->query("SELECT * FROM responses $cond ORDER BY submitted_at DESC")->fetchAll(PDO::FETCH_ASSOC);

        foreach ($resp as &$r) {
            $ans = $pdo->prepare("
                SELECT c.name AS category_name, AVG(a.rating_value) AS avg_rating
                FROM response_answers a
                JOIN questions q ON q.id = a.question_id
                JOIN categories c ON c.id = q.category_id
                WHERE a.response_id = ? GROUP BY c.id
            ");
            $ans->execute([$r['id']]);
            $r['categories'] = $ans->fetchAll(PDO::FETCH_ASSOC);

            $r['overall'] = 0;
            if (count($r['categories'])) {
                $sum = 0;
                foreach ($r['categories'] as $c) {
                    $sum += $c['avg_rating'];
                }
                $r['overall'] = $sum / count($r['categories']);
            }
        }

        jsonOut(["success" => true, "responses" => $resp]);
    } catch (Exception $e) {
        jsonOut(["success" => false, "error" => $e->getMessage()]);
    }
}

/* ========== SUBMIT SURVEY RESPONSE ========== */
if ($_SERVER['REQUEST_METHOD'] === 'POST' && $action === 'submit_survey') {
    try {
        if (
            !$data ||
            !isset($data['categories']) ||
            !is_array($data['categories'])
        ) {
            jsonOut(['success' => false, 'error' => 'Invalid input']);
        }

        // Insert new response
        $stmt = $pdo->prepare("INSERT INTO responses (submitted_at) VALUES (NOW())");
        $stmt->execute();
        $response_id = $pdo->lastInsertId();

        $answer_time = isset($data['answer_time']) ? intval($data['answer_time']) : 0;

        $pdo->prepare("UPDATE responses SET answer_time = ? WHERE id = ?")
            ->execute([$answer_time, $response_id]);

        $total = 0;
        $count = 0;

        // Insert answers
        $ansStmt = $pdo->prepare("INSERT INTO response_answers (response_id, question_id, rating_value) VALUES (?, ?, ?)");

        foreach ($data['categories'] as $cat) {
            if (!isset($cat['questions'])) continue;
            foreach ($cat['questions'] as $q) {
                if (!isset($q['question_id'], $q['rating'])) continue;
                $ansStmt->execute([$response_id, $q['question_id'], $q['rating']]);
                $total += floatval($q['rating']);
                $count++;
            }
        }

        // Compute average
        $overall = $count ? $total / $count : 0;
        $pdo->prepare("UPDATE responses SET overall_score = ? WHERE id = ?")->execute([$overall, $response_id]);

        jsonOut(['success' => true, 'response_id' => $response_id, 'overall' => $overall]);
    } catch (Exception $e) {
        jsonOut(['success' => false, 'error' => $e->getMessage()]);
    }
}

/* ========== DASHBOARD STATS ========== */
if ($action === 'dashboard_stats') {
    try {
        $days = $_GET['days'] ?? null;
        $cond = $days ? "WHERE r.submitted_at >= NOW() - INTERVAL $days DAY" : "";

        $total = $pdo->query("SELECT COUNT(*) FROM responses $cond")->fetchColumn();
        $last = $pdo->query("SELECT MAX(submitted_at) FROM responses")->fetchColumn();

        $avgStmt = $pdo->query("SELECT AVG(answer_time) AS avg_time FROM responses $cond");
        $avgRow = $avgStmt->fetch(PDO::FETCH_ASSOC);
        $average_time = ($avgRow && $avgRow['avg_time'] !== null)
            ? round($avgRow['avg_time'])
            : null;

        $cats = $pdo->query("
            SELECT c.name AS title, AVG(a.rating_value) AS avgv
            FROM response_answers a
            JOIN questions q ON q.id = a.question_id
            JOIN categories c ON c.id = q.category_id
            JOIN responses r ON r.id = a.response_id
            $cond
            GROUP BY c.id
        ")->fetchAll(PDO::FETCH_ASSOC);

        $overall_avg = 0;
        if (count($cats)) {
            $sum = 0;
            foreach ($cats as $c) {
                $sum += $c['avgv'];
            }
            $overall_avg = $sum / count($cats);
        }

        $trend = $pdo->query("
            SELECT DATE(r.submitted_at) AS d, AVG(a.rating_value) AS avgv
            FROM response_answers a
            JOIN responses r ON r.id = a.response_id
            $cond
            GROUP BY DATE(r.submitted_at)
            ORDER BY d ASC
        ")->fetchAll(PDO::FETCH_ASSOC);

        jsonOut([
            "success" => true,
            "total" => $total,
            "overall_avg" => floatval($overall_avg),
            "average_time" => $average_time, 
            "last" => $last,
            "categories" => $cats,
            "trend" => $trend
        ]);
    } catch (Exception $e) {
        jsonOut(["success" => false, "error" => $e->getMessage()]);
    }
}

/* ========== EXPORT CSV ========== */
if ($action === 'export_csv') {
    try {
        header('Content-Type: text/csv');
        header('Content-Disposition: attachment; filename="survey_responses.csv"');
        $out = fopen('php://output', 'w');
        fputcsv($out, ['Response ID', 'Date', 'Category', 'Average']);
        $rows = $pdo->query("
            SELECT r.id, r.submitted_at, c.name AS category, AVG(a.rating_value) AS avg
            FROM responses r
            JOIN response_answers a ON a.response_id = r.id
            JOIN questions q ON q.id = a.question_id
            JOIN categories c ON c.id = q.category_id
            GROUP BY r.id, c.id
        ")->fetchAll(PDO::FETCH_ASSOC);
        foreach ($rows as $r) {
            fputcsv($out, [$r['id'], $r['submitted_at'], $r['category'], round($r['avg'], 2)]);
        }
        fclose($out);
        exit;
    } catch (Exception $e) {
        jsonOut(["success" => false, "error" => $e->getMessage()]);
    }
}

/* ========== DELETE ALL RESPONSES ========== */
if ($action === 'delete_responses') {
    try {
        $pdo->exec("DELETE FROM response_answers");
        $pdo->exec("DELETE FROM responses");
        jsonOut(['success' => true]);
    } catch (Exception $e) {
        jsonOut(['success' => false, 'error' => $e->getMessage()]);
    }
}

jsonOut(["success" => false, "error" => "Invalid action: $action"]);