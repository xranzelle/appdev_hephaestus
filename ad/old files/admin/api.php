<?php
error_reporting(E_ALL);
ini_set('display_errors', 1);

header('Content-Type: application/json');
header('Access-Control-Allow-Origin: *');
header('Access-Control-Allow-Methods: GET, POST, OPTIONS');
header('Access-Control-Allow-Headers: Content-Type');

if ($_SERVER['REQUEST_METHOD'] === 'OPTIONS') {
    // CORS preflight
    exit(0);
}

// ===== DATABASE CONNECTION =====
$host = "srv613.hstgr.io";
$dbname = "u412048963_survey_db";
$user = "u412048963_hephaestus";
$pass = "Hepastu5!";

// ===== DATABASE CONNECTION =====
$host = "srv613.hstgr.io";
$dbname = "u412048963_survey_db";
$user = "u412048963_hephaestus";
$pass = "Hepastu5!";

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
        'error' => $e->getMessage()
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
    $cats = $pdo->query("SELECT * FROM categories ORDER BY id ASC")->fetchAll(PDO::FETCH_ASSOC);
    foreach ($cats as &$c) {
        $q = $pdo->prepare("SELECT * FROM questions WHERE category_id = ? ORDER BY id ASC");
        $q->execute([$c['id']]);
        $c['questions'] = $q->fetchAll(PDO::FETCH_ASSOC);
    }
    jsonOut(["categories" => $cats]);
}

if ($action === 'add_category') {
    $title = trim($data['title'] ?? '');
    if ($title === '') {
        jsonOut(["error" => "No title provided"]);
    }
    $stmt = $pdo->prepare("INSERT INTO categories (name) VALUES (?)");
    $stmt->execute([$title]);
    jsonOut(["success" => true]);
}

if ($action === 'update_category') {
    $stmt = $pdo->prepare("UPDATE categories SET name = ? WHERE id = ?");
    $stmt->execute([$data['title'], $data['id']]);
    jsonOut(["success" => true]);
}

if ($action === 'delete_category') {
    $stmt = $pdo->prepare("DELETE FROM categories WHERE id = ?");
    $stmt->execute([$data['id']]);
    jsonOut(["success" => true]);
}



/* ========== QUESTION CRUD ========== */
if ($action === 'add_question') {
    $stmt = $pdo->prepare("INSERT INTO questions (category_id, question_text) VALUES (?, ?)");
    $stmt->execute([$data['category_id'], $data['text']]);
    jsonOut(["success" => true]);
}

if ($action === 'update_question') {
    $stmt = $pdo->prepare("UPDATE questions SET question_text = ? WHERE id = ?");
    $stmt->execute([$data['text'], $data['id']]);
    jsonOut(["success" => true]);
}

if ($action === 'delete_question') {
    $stmt = $pdo->prepare("DELETE FROM questions WHERE id = ?");
    $stmt->execute([$data['id']]);
    jsonOut(["success" => true]);
}

/* ========== RESPONSES FETCH ========== */
if ($action === 'get_responses') {
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

    jsonOut(["responses" => $resp]);
}

/* ========== DASHBOARD STATS ========== */
if ($action === 'dashboard_stats') {
    $days = $_GET['days'] ?? null;
    $cond = $days ? "WHERE r.submitted_at >= NOW() - INTERVAL $days DAY" : "";

    $total = $pdo->query("SELECT COUNT(*) FROM responses $cond")->fetchColumn();
    $last = $pdo->query("SELECT MAX(submitted_at) FROM responses")->fetchColumn();

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
        "total" => $total,
        "overall_avg" => floatval($overall_avg),
        "last" => $last,
        "categories" => $cats,
        "trend" => $trend
    ]);
}

/* ========== EXPORT CSV ========== */
if ($action === 'export_csv') {
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
}

if ($_GET['action'] === 'delete_responses' || $_POST['action'] === 'delete_responses') {
    $pdo->exec("DELETE FROM responses");
    echo json_encode(['success' => true]);
    exit;
}


jsonOut(["error" => "Invalid action"]);
