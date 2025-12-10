<?php
session_start();
error_reporting(E_ALL);
ini_set('display_errors', 1);

// Database config
$host = "srv613.hstgr.io";
$dbname = "u412048963_survey_db";
$user = "u412048963_hephaestus";
$pass = "Hepastu5!";

try {
    $pdo = new PDO(
        "mysql:host=$host;dbname=$dbname;charset=utf8mb4",
        $user,
        $pass,
        [PDO::ATTR_ERRMODE => PDO::ERRMODE_EXCEPTION]
    );
} catch (Exception $e) {
    die("DB Error: " . $e->getMessage());
}

$error = "";

if ($_SERVER["REQUEST_METHOD"] === "POST") {

    $username = trim($_POST["username"]);
    $password = trim($_POST["password"]);

    /* ============================================================
        HARD-CODED ACCOUNTS (WITH ROLES)
    ============================================================ */
    $hardcoded = [
        "superadmin" => ["password" => "super123", "role" => "superadmin"],
        "admin"      => ["password" => "admin123", "role" => "admin"],
        "viewer"     => ["password" => "view123",  "role" => "viewer"]
    ];

    if (isset($hardcoded[$username]) && $password === $hardcoded[$username]["password"]) {

        $_SESSION["admin_logged_in"] = true;
        $_SESSION["admin_username"]  = $username;
        $_SESSION["admin_role"]      = $hardcoded[$username]["role"];

        header("Location: dashboard.php");
        exit;
    }

    /* ============================================================
        DATABASE LOGIN (OPTIONAL)
    ============================================================ */
    $stmt = $pdo->prepare("SELECT * FROM admin_accounts WHERE username = ?");
    $stmt->execute([$username]);
    $admin = $stmt->fetch(PDO::FETCH_ASSOC);

    if ($admin && password_verify($password, $admin["password_hash"])) {

        $_SESSION["admin_logged_in"] = true;
        $_SESSION["admin_username"]  = $admin["username"];
        $_SESSION["admin_role"]      = "viewer";
        $_SESSION["full_name"]       = $admin["full_name"];

        header("Location: dashboard.php");
        exit;
    } else {
        $error = "Invalid username or password.";
    }
}
?>
<!DOCTYPE html>
<html>
<head>
    <title>Admin Login</title>
    <link rel="stylesheet" href="admin.css">
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.5.0/css/all.min.css"/>
    <style>
        .login-box {
            width: 350px;
            margin: 120px auto;
            background: var(--card);
            padding: 24px;
            border-radius: 12px;
            box-shadow: 0 6px 18px rgba(12,24,40,0.08);
        }
        input {
            width: 100%;
            padding: 12px;
            border-radius: 8px;
            margin-top: 10px;
            border: 1px solid #dce3ef;
        }
        .login-btn {
            background: var(--accent);
            color: #fff;
            font-weight: 700;
            border: none;
            padding: 12px;
            width: 100%;
            border-radius: 8px;
            margin-top: 14px;
            cursor: pointer;
            font-size: 14px;
        }
        .signup-btn {
            display:block;
            margin-top:10px;
            padding:12px;
            width:100%;
            background:#6b7280;
            border-radius:8px;
            color:white;
            text-align:center;
            font-weight:600;
            text-decoration:none;
            font-size: 14px;
        }
        .error {
            padding:10px;
            background:rgba(194,59,59,0.1);
            color:var(--danger);
            text-align:center;
            margin-bottom:10px;
            border-radius:6px;
            border:1px solid rgba(194,59,59,0.15);
        }
        h2 { text-align:center; margin-bottom:10px; }

        .password-wrapper { position: relative; }
        .toggle-pass {
            position: absolute;
            right: 12px;
            top: 50%;
            transform: translateY(-50%);
            cursor: pointer;
            user-select: none;
            color: var(--muted);
            font-size: 18px;
        }
    </style>
</head>
<body>

<div class="login-box">
    <h2>Admin Login</h2>

    <?php if ($error): ?>
        <div class="error"><?= htmlspecialchars($error) ?></div>
    <?php endif; ?>

    <form method="POST">
        <input type="text" name="username" placeholder="Username" required />

        <div class="password-wrapper">
            <input type="password" name="password" id="password" placeholder="Password" required />
            <span class="toggle-pass"><i class="fa-solid fa-eye" id="eye-icon"></i></span>
        </div>

        <button class="login-btn">Login</button>
    </form>

    <a class="signup-btn" href="signup.php">Create Admin Account</a>
</div>

<script>
    const togglePass = document.querySelector('.toggle-pass');
    const passwordInput = document.getElementById('password');
    const eyeIcon = document.getElementById('eye-icon');

    togglePass.addEventListener('click', () => {
        if (passwordInput.type === 'password') {
            passwordInput.type = 'text';
            eyeIcon.classList.replace('fa-eye', 'fa-eye-slash');
        } else {
            passwordInput.type = 'password';
            eyeIcon.classList.replace('fa-eye-slash', 'fa-eye');
        }
    });
</script>

</body>
</html>