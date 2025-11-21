<?php
error_reporting(E_ALL);
ini_set('display_errors', 0);
header('Content-Type: application/json');
header('Access-Control-Allow-Origin: *');
header('Access-Control-Allow-Methods: GET, POST, OPTIONS');
header('Access-Control-Allow-Headers: Content-Type');

if ($_SERVER['REQUEST_METHOD'] === 'OPTIONS') {
    exit(0);
}

// ===== DATABASE CONNECTION =====
$host = "srv613.hstgr.io";
$dbname = "u412048963_survey_db";
$user = "u412048963_hephaestus";
$pass = "Hepastu5!";

try {
    // ✅ Create PDO connection
    $pdo = new PDO(
        "mysql:host=$host;dbname=$dbname;charset=utf8mb4",
        $user,
        $pass,
        [
            PDO::ATTR_ERRMODE => PDO::ERRMODE_EXCEPTION,
            PDO::ATTR_DEFAULT_FETCH_MODE => PDO::FETCH_ASSOC,
        ]
    );

    // ✅ Force both PHP and MySQL to use Philippine time
    date_default_timezone_set('Asia/Manila');
    $pdo->exec("SET time_zone = '+08:00'");

} catch (Exception $e) {
    // ❌ Handle connection or query errors safely
    die(json_encode([
        'success' => false,
        'error' => $e->getMessage()
    ]));
}

// ===== HANDLE ACTION =====
$action = $_GET['action'] ?? '';
$input = json_decode(file_get_contents('php://input'), true);

// ===== FETCH SURVEY STRUCTURE =====
if ($action === 'get_survey') {
    try {
        $categories = $pdo->query("SELECT * FROM categories ORDER BY id ASC")->fetchAll(PDO::FETCH_ASSOC);

        foreach ($categories as &$cat) {
            $q = $pdo->prepare("SELECT * FROM questions WHERE category_id = ? ORDER BY id ASC");
            $q->execute([$cat['id']]);
            $cat['questions'] = $q->fetchAll(PDO::FETCH_ASSOC);
        }

        echo json_encode(['success' => true, 'categories' => $categories]);
        exit;
    } catch (Exception $e) {
        echo json_encode(['success' => false, 'error' => $e->getMessage()]);
        exit;
    }
}

// ===== SUBMIT SURVEY RESPONSE =====
if ($_SERVER['REQUEST_METHOD'] === 'POST' && $action === 'submit_survey') {
    try {
        if (
            !$input ||
            !isset($input['categories']) ||
            !is_array($input['categories'])
        ) {
            echo json_encode(['success' => false, 'error' => 'Invalid input']);
            exit;
        }

        // 1️⃣ Insert new response
        $stmt = $pdo->prepare("INSERT INTO responses (submitted_at) VALUES (NOW())");
        $stmt->execute();
        $response_id = $pdo->lastInsertId();

        $total = 0;
        $count = 0;

        // 2️⃣ Insert answers
        $ansStmt = $pdo->prepare("INSERT INTO response_answers (response_id, question_id, rating_value) VALUES (?, ?, ?)");

        foreach ($input['categories'] as $cat) {
            if (!isset($cat['questions'])) continue;
            foreach ($cat['questions'] as $q) {
                if (!isset($q['question_id'], $q['rating'])) continue;
                $ansStmt->execute([$response_id, $q['question_id'], $q['rating']]);
                $total += floatval($q['rating']);
                $count++;
            }
        }

        // 3️⃣ Compute average
        $overall = $count ? $total / $count : 0;
        $pdo->prepare("UPDATE responses SET overall_score = ? WHERE id = ?")->execute([$overall, $response_id]);

        echo json_encode(['success' => true, 'response_id' => $response_id, 'overall' => $overall]);
        exit;
    } catch (Exception $e) {
        echo json_encode(['success' => false, 'error' => $e->getMessage()]);
        exit;
    }
}

// ===== FETCH ALL RESPONSES (for analytics) =====
if ($action === 'get_responses') {
    try {
        $rows = $pdo->query("
            SELECT r.id, r.submitted_at, r.overall_score
            FROM responses r
            ORDER BY r.submitted_at DESC
        ")->fetchAll(PDO::FETCH_ASSOC);

        echo json_encode(['success' => true, 'data' => $rows]);
        exit;
    } catch (Exception $e) {
        echo json_encode(['success' => false, 'error' => $e->getMessage()]);
        exit;
    }
}

// ===== INVALID REQUEST =====
echo json_encode(['success' => false, 'error' => 'Invalid action or request method']);
exit;
?>
