<?php
session_start();

// Prevent access if not logged in
if (!isset($_SESSION["admin_logged_in"]) || $_SESSION["admin_logged_in"] !== true) {
    header("Location: login.php");
    exit;
}

$admin_username = $_SESSION["admin_username"] ?? "Admin";
?>
<!doctype html>
<html lang="en">
<head>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width,initial-scale=1" />
    <title>Admin Dashboard — Survey Manager</title>

    <!-- Chart.js & jsPDF CDN -->
    <script src="https://cdn.jsdelivr.net/npm/chart.js"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/jspdf/2.5.1/jspdf.umd.min.js"></script>
    <link rel="stylesheet" href="admin.css">

    <!-- Google Fonts: Poppins for Logout Modal only -->
    <link href="https://fonts.googleapis.com/css2?family=Poppins:wght@400;500;600;700&display=swap" rel="stylesheet">

    <style>
        /* Logout Modal Styles with Poppins font */
        #logoutModal {
            display: none;
            position: fixed;
            z-index: 1000;
            left: 0;
            top: 0;
            width: 100%;
            height: 100%;
            overflow: auto;
            background-color: rgba(0,0,0,0.5);
        }

        #logoutModal .modal-content {
            background-color: #fff;
            margin: 15% auto;
            padding: 20px;
            border-radius: 8px;
            width: 300px;
            text-align: center;
            font-family: 'Poppins', sans-serif; /* Only here */
        }

        #logoutModal .modal-buttons {
            margin-top: 20px;
            display: flex;
            justify-content: space-around;
        }

        #logoutModal .btn {
            padding: 8px 16px;
            border: none;
            border-radius: 4px;
            cursor: pointer;
            font-weight: 500;
        }

        #logoutModal .btn.cancel {
            background-color: #ccc;
            color: #000;
        }

        #logoutModal .btn.confirm {
            background-color: #e74c3c;
            color: #fff;
        }
    </style>
</head>
<body>
<div class="app">
    <aside class="sidebar">
        <div class="brand">Survey Admin Panel</div>
        <nav class="nav">
            <button id="nav-dashboard" class="active">Dashboard</button>
            <button id="nav-analytics">Analytics</button>
            <button id="nav-manage">Manage Survey</button>
            <button id="nav-responses">Responses</button>

            <!-- Logout -->
            <form id="logoutForm" action="logout.php" method="POST" style="margin-top:18px;">
                <button type="button" id="nav-logout" class="btn danger" style="width:100%;">Logout</button>
            </form>
        </nav>
        <div style="margin-top:18px;font-size:12px;color:var(--muted)">
            Connected to MySQL via api.php
        </div>
    </aside>

    <main class="main">
        <header class="topbar">
            <div class="top-left">Customer Satisfaction — Admin</div>
            <div style="color:var(--muted);font-size:13px;">
                Signed in as <strong><?= htmlspecialchars($admin_username) ?></strong>
            </div>
        </header>

        <!-- DASHBOARD -->
        <section id="page-dashboard">
            <div class="grid-3">
                <div class="card">
                    <div>Total Responses</div>
                    <h2 id="stat-count">0</h2>
                </div>
                <div class="card">
                    <div>Average Score</div>
                    <h2 id="stat-avg">0</h2>
                </div>
                <div class="card">
                    <div>Average Time</div>
                    <h2 id="stat-time">—</h2>
                </div>
                <div class="card">
                    <div>Last Response</div>
                    <h2 id="stat-last">—</h2>
                </div>
            </div>

            <div class="card">
                <div style="display:flex;justify-content:space-between;align-items:center;margin-bottom:12px;">
                    <h3>Average by Category</h3>
                    <div>
                        <select id="filterCategory">
                            <option value="all">All Categories</option>
                        </select>
                        <select id="filterTime">
                            <option value="all">All Time</option>
                            <option value="today">Today</option>
                            <option value="yesterday">Yesterday</option>
                            <option value="7">Last 7 Days</option>
                            <option value="30">Last 30 Days</option>
                        </select>
                        <button id="downloadCSV" class="btn ghost">Export CSV</button>
                    </div>
                </div>
                <canvas id="barChart" height="120"></canvas>
            </div>

            <div class="card">
                <h3>Overall Average Trend</h3>
                <canvas id="lineChart" height="120"></canvas>
            </div>

            <div class="card">
                <h3>Category Contribution</h3>
                <canvas id="pieChart" height="120"></canvas>
            </div>
        </section>

        <!-- ANALYTICS -->
        <section id="page-analytics" style="display:none">
            <div class="card">
                <h3>Summary Report</h3>
                <p>View aggregated insights and export as PDF.</p>
                <div style="margin-bottom:12px;">
                    <button id="exportReport" class="btn">Export PDF Report</button>
                </div>
                <canvas id="analyticsChart" height="160"></canvas>
            </div>
            <div class="card">
                <h3>Category Trends Over Time</h3>
                <canvas id="categoryTrendChart" height="160"></canvas>
            </div>
        </section>

        <!-- MANAGE -->
        <section id="page-manage" style="display:none">
            <div class="card">
                <h3>Create New Category</h3>
                <div style="display:flex;gap:8px">
                    <input id="newCategoryName" placeholder="Category name">
                    <button id="addCategoryBtn" class="btn">Add Category</button>
                </div>
            </div>
            <div class="card">
                <h3>Add Question</h3>
                <div style="display:flex;gap:8px;align-items:center">
                    <select id="selectCategoryToAdd"></select>
                    <input id="newQuestionText" placeholder="Question text">
                    <button id="addQuestionBtn" class="btn">Add Question</button>
                </div>
            </div>
            <div class="card">
                <h3>Categories & Questions</h3>
                <div id="categoryList" class="category-list"></div>
            </div>
        </section>

        <!-- RESPONSES -->
        <section id="page-responses" style="display:none">
            <div style="margin-top:12px;display:flex;justify-content:flex-end;gap:8px">
                <button id="clearResponses" class="btn danger">Clear All Responses</button>
            </div>
            <div class="card">
                <div style="display:flex;justify-content:space-between;align-items:center">
                    <h3>Responses</h3>
                    <div>
                        <select id="respFilterTime">
                            <option value="all">All Time</option>
                            <option value="today">Today</option>
                            <option value="yesterday">Yesterday</option>
                            <option value="7">Last 7 Days</option>
                            <option value="30">Last 30 Days</option>
                        </select>
                    </div>
                </div>
                <div style="margin-top:12px;overflow:auto;">
                    <table>
                        <thead>
                        <tr>
                            <th>Date</th>
                            <th>Overall</th>
                            <th>Details</th>
                        </tr>
                        </thead>
                        <tbody id="responsesTableBody"></tbody>
                    </table>
                </div>
            </div>
        </section>
    </main>
</div>

<!-- Logout Modal -->
<div id="logoutModal">
    <div class="modal-content">
        <h3>Confirm Logout</h3>
        <p>Are you sure you want to log out?</p>
        <div class="modal-buttons">
            <button class="btn cancel" id="cancelLogout">Cancel</button>
            <button class="btn confirm" id="confirmLogout">Logout</button>
        </div>
    </div>
</div>

<script src="admin-script.js" defer></script>

<script>
    const logoutBtn = document.getElementById('nav-logout');
    const logoutModal = document.getElementById('logoutModal');
    const cancelBtn = document.getElementById('cancelLogout');
    const confirmBtn = document.getElementById('confirmLogout');
    const logoutForm = document.getElementById('logoutForm');

    // Show modal on logout click
    logoutBtn.addEventListener('click', () => {
        logoutModal.style.display = 'block';
    });

    // Cancel logout
    cancelBtn.addEventListener('click', () => {
        logoutModal.style.display = 'none';
    });

    // Confirm logout
    confirmBtn.addEventListener('click', () => {
        logoutForm.submit();
    });

    // Close modal if clicked outside
    window.addEventListener('click', (e) => {
        if (e.target === logoutModal) {
            logoutModal.style.display = 'none';
        }
    });
</script>

</body>
</html>
