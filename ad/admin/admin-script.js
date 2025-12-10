const API_URL = 'api.php';

const pages = {
    dashboard: document.getElementById('page-dashboard'),
    analytics: document.getElementById('page-analytics'),
    manage: document.getElementById('page-manage'),
    responses: document.getElementById('page-responses'),
};

const nav = {
    dashboard: document.getElementById('nav-dashboard'),
    analytics: document.getElementById('nav-analytics'),
    manage: document.getElementById('nav-manage'),
    responses: document.getElementById('nav-responses')
};

function showPage(key) {
    for (const p in pages) pages[p].style.display = (p === key ? 'block' : 'none');
    document.querySelectorAll('.nav button').forEach(b => b.classList.remove('active'));
    nav[key].classList.add('active');

    if (key === 'dashboard') renderDashboard();
    if (key === 'analytics') renderAnalytics();
    if (key === 'manage') renderManage();
    if (key === 'responses') renderResponses();
}

Object.keys(nav).forEach(k => nav[k].onclick = () => showPage(k));

// Helper function to format seconds into readable time
function formatTime(seconds) {
    if (!seconds || seconds === 0) return "—";

    const hours = Math.floor(seconds / 3600);
    const minutes = Math.floor((seconds % 3600) / 60);
    const secs = Math.floor(seconds % 60);

    if (hours > 0) {
        return `${hours} hour${hours > 1 ? 's' : ''} ${minutes} min${minutes !== 1 ? 's' : ''}`;
    } else if (minutes > 0) {
        return `${minutes} min${minutes !== 1 ? 's' : ''} ${secs} sec${secs !== 1 ? 's' : ''}`;
    } else {
        return `${secs} second${secs !== 1 ? 's' : ''}`;
    }
}

// utility fetch wrapper
async function api(action, method = 'GET', payload = null) {
    let url = API_URL + '?action=' + encodeURIComponent(action);
    if (method === 'GET' && payload && Object.keys(payload).length) {
        url += '&' + new URLSearchParams(payload).toString();
    }
    const opts = { method };
    if (method !== 'GET' && payload !== null) {
        opts.headers = { 'Content-Type': 'application/json' };
        opts.body = JSON.stringify(payload);
    } else if (method !== 'GET' && method !== 'DELETE' && payload === null) {
        opts.headers = { 'Content-Type': 'application/json' };
        opts.body = '{}';
    }
    const res = await fetch(url, opts);
    const ct = res.headers.get('Content-Type') || '';
    if (ct.includes('application/json')) {
        return res.json();
    }
    return res.text();
}

// get survey structure
async function loadSurvey() {
    const r = await api('get_survey');
    return r.categories || [];
}

// load responses with optional days filter
async function loadResponses(days = null) {
    const params = {};
    if (days && days !== 'all') params.days = days;
    const r = await api('get_responses', 'GET', params);
    return r.responses || [];
}

// Chart instances
let barChart, lineChart, pieChart, analyticsChart;

// render dashboard
let cachedStats = null;
let cachedResponses = null;

async function renderDashboard() {

    const days = document.getElementById('filterTime').value;
    const params = (days && days !== 'all') ? { days } : {};

    // ===============================
    // CACHE STATS ONLY
    // ===============================
    if (!cachedStats || days !== cachedStats._days) {
        console.log("Fetching dashboard stats...");
        cachedStats = await api('dashboard_stats', 'GET', params);
        cachedStats._days = days;
    } else {
        console.log("Using cached stats");
    }

    const stats = cachedStats;

    // ===============================
    // CHECK NO DATA
    // ===============================
    const noData = stats.total === 0 || !stats.categories || stats.categories.length === 0;

    // ALWAYS UPDATE UI kahit walang data
    document.getElementById('stat-count').innerText = stats.total;
    document.getElementById('stat-avg').innerText = noData ? '0.00' : stats.overall_avg.toFixed(2);
    document.getElementById('stat-time').innerText = noData ? '0s' : formatTime(stats.average_time);
    document.getElementById('stat-last').innerText = stats.last ? new Date(stats.last).toLocaleString() : '—';

    const catSelect = document.getElementById('filterCategory');
    catSelect.innerHTML =
        '<option value="all">All Categories</option>' +
        (stats.categories || []).map(c => `<option value="${c.title}">${c.title}</option>`).join('');

    // ===============================
    // 🚫 IF NO DATA → CLEAR GRAPHS
    // ===============================

    if (noData) {

        if (barChart) barChart.destroy();
        if (lineChart) lineChart.destroy();
        if (pieChart) pieChart.destroy();

        // BAR CHART NO DATA
        let b = document.getElementById('barChart').getContext('2d');
        b.clearRect(0, 0, b.canvas.width, b.canvas.height);
        b.font = '16px Poppins';
        b.fillStyle = '#999';
        b.textAlign = 'center';
        b.fillText('No data available', b.canvas.width / 2, b.canvas.height / 2);

        // LINE CHART NO DATA
        let l = document.getElementById('lineChart').getContext('2d');
        l.clearRect(0, 0, l.canvas.width, l.canvas.height);
        l.font = '16px Poppins';
        l.fillStyle = '#999';
        l.textAlign = 'center';
        l.fillText('No trend data', l.canvas.width / 2, l.canvas.height / 2);

        // PIE CHART NO DATA
        let p = document.getElementById('pieChart').getContext('2d');
        p.clearRect(0, 0, p.canvas.width, p.canvas.height);
        p.font = '16px Poppins';
        p.fillStyle = '#999';
        p.textAlign = 'center';
        p.fillText('No category data', p.canvas.width / 2, p.canvas.height / 2);

        return;
    }

    // ===============================
    // IF DATA EXISTS → NORMAL CHARTS
    // ===============================

    const allLabels = stats.categories.map(c => c.title);
    const allVals = stats.categories.map(c => Number(parseFloat(c.avgv).toFixed(2)));

    // Bar chart
    if (barChart) barChart.destroy();
    barChart = new Chart(document.getElementById('barChart'), {
        type: 'bar',
        data: { labels: allLabels, datasets: [{ label: 'Avg', data: allVals, backgroundColor: '#1f6fb2' }] },
        options: { scales: { y: { beginAtZero: true, max: 5 } }, plugins: { legend: { display: false } } }
    });

    // Category filter
    catSelect.onchange = () => {
        const selectedCat = catSelect.value;

        if (selectedCat === 'all') {
            barChart.data.labels = allLabels;
            barChart.data.datasets[0].data = allVals;
        } else {
            const match = stats.categories.find(c => c.title === selectedCat);
            barChart.data.labels = [selectedCat];
            barChart.data.datasets[0].data = [match ? Number(match.avgv) : 0];
        }
        barChart.update();
    };

    // Trend line chart
    let trendLabels = stats.trend.map(t => t.d).sort((a, b) => new Date(a) - new Date(b));
    let trendVals = stats.trend.map(t => Number(parseFloat(t.avgv).toFixed(2)));

    const limit = 7;
    trendLabels = trendLabels.slice(-limit);
    trendVals = trendVals.slice(-limit);

    if (lineChart) lineChart.destroy();
    lineChart = new Chart(document.getElementById('lineChart'), {
        type: 'line',
        data: { labels: trendLabels, datasets: [{ label: 'Overall Avg', data: trendVals, borderColor: '#1f8f7a', fill: false }] },
        options: { scales: { y: { beginAtZero: true, max: 5 } }, plugins: { legend: { display: false } } }
    });

    // Pie chart
    if (pieChart) pieChart.destroy();
    pieChart = new Chart(document.getElementById('pieChart'), {
        type: 'pie',
        data: {
            labels: allLabels,
            datasets: [{
                data: allVals,
                backgroundColor: ['#1f6fb2', '#1f8f7a', '#f59e0b', '#e11d48', '#8b5cf6']
            }]
        },
        options: { plugins: { legend: { position: 'bottom' } } }
    });

    renderAnalytics();
}


// render analytics with number of respondents per day
async function renderAnalytics() {
    if (!cachedResponses) {
        console.log("Fetching responses from API...");
        cachedResponses = await loadResponses(null);
    } else {
        console.log("Using cached responses");
    }

    const responses = cachedResponses;
    const LIMIT = 7;

    // ========= FAST GROUPING =========
    const map = {};
    const catMap = {};

    for (const r of responses) {
        const d = new Date(r.submitted_at).toLocaleDateString('en-CA', { timeZone: 'Asia/Manila' });

        // Overall day summary
        if (!map[d]) map[d] = { sum: 0, count: 0 };
        map[d].sum += Number(r.overall) || 0;
        map[d].count++;

        // Category summary
        if (r.categories) {
            for (const c of r.categories) {
                const cat = c.category_name;

                if (!catMap[cat]) catMap[cat] = {};
                if (!catMap[cat][d]) catMap[cat][d] = { sum: 0, count: 0 };

                catMap[cat][d].sum += Number(c.avg_rating) || 0;
                catMap[cat][d].count++;
            }
        }
    }

    // ========= BAR CHART =========
    let labels = Object.keys(map).sort((a, b) => new Date(a) - new Date(b));
    labels = labels.slice(-LIMIT);

    const avgVals = labels.map(l => map[l].sum / map[l].count);
    const countVals = labels.map(l => map[l].count);

    if (window.analyticsChart instanceof Chart) window.analyticsChart.destroy();

    window.analyticsChart = new Chart(document.getElementById('analyticsChart'), {
        type: 'bar',
        data: {
            labels,
            datasets: [
                { label: 'Average Score', data: avgVals, backgroundColor: '#1f6fb2', yAxisID: 'y1' },
                { label: 'Respondents', data: countVals, backgroundColor: '#f59e0b', yAxisID: 'y2' }
            ]
        },
        options: {
            responsive: true,
            scales: {
                y1: { type: 'linear', position: 'left', beginAtZero: true, max: 5 },
                y2: { type: 'linear', position: 'right', beginAtZero: true, grid: { drawOnChartArea: false } }
            }
        }
    });

    // ========= CATEGORY TREND =========
    let allDates = [
        ...new Set(
            Object.values(catMap).flatMap(entry => Object.keys(entry))
        )
    ].sort((a, b) => new Date(a) - new Date(b));

    allDates = allDates.slice(-LIMIT);

    const colors = ['#1f6fb2', '#e11d48', '#f59e0b', '#1f8f7a', '#8b5cf6', '#9333ea', '#ef4444', '#14b8a6'];

    const datasets = Object.keys(catMap).map((cat, i) => ({
        label: cat,
        data: allDates.map(d => catMap[cat][d] ? (catMap[cat][d].sum / catMap[cat][d].count) : null),
        borderColor: colors[i % colors.length],
        tension: 0.3,
        fill: false,
        spanGaps: true
    }));

    if (window.categoryTrendChart instanceof Chart) window.categoryTrendChart.destroy();

    window.categoryTrendChart = new Chart(document.getElementById('categoryTrendChart'), {
        type: 'line',
        data: { labels: allDates, datasets },
        options: {
            responsive: true,
            plugins: { title: { display: true, text: 'Category Average Trend' } },
            scales: {
                x: { title: { text: 'Date', display: true } },
                y: { beginAtZero: true, max: 5 }
            }
        }
    });
}

// attach filter change
document.getElementById('respFilterTime').onchange = renderAnalytics;

// manage: render categories & questions
async function renderManage() {
    const categories = await loadSurvey();
    const catList = document.getElementById('categoryList');
    catList.innerHTML = '';
    const selectAdd = document.getElementById('selectCategoryToAdd');
    selectAdd.innerHTML = categories.map(c => `<option value="${c.id}">${escapeHtml(c.name)}</option>`).join('');

    categories.forEach(cat => {
        const div = document.createElement('div');
        div.className = 'card';
        div.innerHTML = `
      <div style="display:flex;justify-content:space-between;align-items:center">
        <strong>${escapeHtml(cat.name)}</strong>
        <div>
          <button data-id="${cat.id}" class="edit-cat btn ghost">Edit</button>
          <button data-id="${cat.id}" class="del-cat btn danger">Delete</button>
        </div>
      </div>
      <div style="margin-top:8px">
        <strong>Questions</strong>
        <div id="qlist-${cat.id}" style="margin-top:8px"></div>
      </div>
    `;
        catList.appendChild(div);
        const qlist = div.querySelector(`#qlist-${cat.id}`);
        (cat.questions || []).forEach(q => {
            const row = document.createElement('div');
            row.style.display = 'flex'; row.style.gap = '8px'; row.style.marginBottom = '6px';
            row.innerHTML = `
        <input value="${escapeHtml(q.question_text)}" style="flex:1" />
        <button class="save-q btn ghost" data-qid="${q.id}" data-cid="${cat.id}">Save</button>
        <button class="del-q btn danger" data-qid="${q.id}">Delete</button>
      `;
            qlist.appendChild(row);
        });
    });

    // attach handlers
    document.querySelectorAll('.del-cat').forEach(b => {
        b.onclick = async (e) => {
            if (!confirm('Delete category and questions?')) return;
            await api('delete_category', 'POST', { id: b.dataset.id });
            renderManage();
            renderDashboard();
        };
    });
    document.querySelectorAll('.edit-cat').forEach(b => {
        b.onclick = async (e) => {
            const id = b.dataset.id;
            const newTitle = prompt('New category title:');
            if (!newTitle) return;
            await api('update_category', 'POST', { id, title: newTitle });
            renderManage();
            renderDashboard();
        };
    });
    document.querySelectorAll('.save-q').forEach(b => {
        b.onclick = async (e) => {
            const qid = b.dataset.qid;
            const input = b.parentElement.querySelector('input');
            await api('update_question', 'POST', { id: qid, text: input.value });
            alert('Saved');
            renderManage();
        };
    });
    document.querySelectorAll('.del-q').forEach(b => {
        b.onclick = async (e) => {
            if (!confirm('Delete question?')) return;
            await api('delete_question', 'POST', { id: b.dataset.qid });
            renderManage();
        };
    });
    document.getElementById('clearResponses').onclick = async () => {
        if (!confirm("Are you sure you want to delete ALL responses? This cannot be undone.")) return;

        try {
            // call API to delete all responses
            await api('delete_responses', 'POST', {});
            alert("All responses have been deleted.");
            renderResponses();
            renderDashboard();
            renderAnalytics();
        } catch (err) {
            console.error(err);
            alert("Failed to delete responses.");
        }
    };
}

let cachedResponsesTable = {};

async function renderResponses() {
    const days = document.getElementById('respFilterTime').value;
    const key = days || 'all';

    // Use cached table if available
    if (cachedResponsesTable[key]) {
        document.getElementById('responsesTableBody').innerHTML = cachedResponsesTable[key];
        console.log(" Using cached responses table");
        return;
    }

    console.log("Fetching responses for table...");

    const responses = await loadResponses(days === 'all' ? null : days);
    const tbody = document.getElementById('responsesTableBody');

    // No responses case
    if (!responses.length) {
        const html = '<tr><td colspan="3">No responses</td></tr>';
        tbody.innerHTML = html;
        cachedResponsesTable[key] = html;
        return;
    }

    // Date cache (avoid repeated new Date parsing)
    const dateCache = Object.create(null);

    // BUILD ONE BIG HTML STRING (FASTEST)
    let html = "";

    // Faster than reverse(): loop backward
    for (let i = responses.length - 1; i >= 0; i--) {
        const r = responses[i];

        // cache date format
        let dateStr = dateCache[r.submitted_at];
        if (!dateStr) {
            dateStr = dateCache[r.submitted_at] =
                new Date(r.submitted_at).toLocaleString('en-US', { hour12: true });
        }

        // category details
        let details = "";
        if (r.categories && r.categories.length) {
            const parts = new Array(r.categories.length);
            for (let j = 0; j < r.categories.length; j++) {
                const c = r.categories[j];
                parts[j] = `${c.category_name}: ${Number(c.avg_rating || 0).toFixed(2)}`;
            }
            details = parts.join(" | ");
        }

        html += `
            <tr>
                <td>${dateStr}</td>
                <td>${(r.overall || 0).toFixed(2)}</td>
                <td style="font-size:12px;color:#555">${details}</td>
            </tr>
        `;
    }
    tbody.innerHTML = html;

    cachedResponsesTable[key] = html;
}

/* Helpers */
function escapeHtml(s) { return String(s || '').replace(/&/g, '&amp;').replace(/</g, '&lt;').replace(/>/g, '&gt;'); }

/* CRUD form handlers */
document.getElementById('addCategoryBtn').onclick = async () => {
    const title = document.getElementById('newCategoryName').value.trim();
    if (!title) return alert('Enter title');
    await api('add_category', 'POST', { title });
    document.getElementById('newCategoryName').value = '';
    renderManage(); renderDashboard();
};

document.getElementById('addQuestionBtn').onclick = async () => {
    const catId = document.getElementById('selectCategoryToAdd').value;
    const text = document.getElementById('newQuestionText').value.trim();
    if (!text) return alert('Enter question text');
    await api('add_question', 'POST', { category_id: catId, text });
    document.getElementById('newQuestionText').value = '';
    renderManage(); renderDashboard();
};

/* Filter & export */
document.getElementById('filterTime').onchange = renderDashboard;
document.getElementById('filterCategory').onchange = async () => {
    const cat = document.getElementById('filterCategory').value;
    const days = document.getElementById('filterTime').value;
    if (cat === 'all') return renderDashboard();
    const responses = await loadResponses(days === 'all' ? null : days);
    const map = {};
    responses.forEach(r => {
        (r.categories || []).forEach(c => {
            if (c.category_name === cat) {
                if (!map[c.category_name]) map[c.category_name] = { sum: 0, count: 0 };
                map[c.category_name].sum += c.avg_rating;
                map[c.category_name].count++;
            }
        });
    });
    const labels = Object.keys(map);
    const vals = labels.map(k => map[k].count ? (map[k].sum / map[k].count) : 0);
    if (barChart) barChart.destroy();
    barChart = new Chart(document.getElementById('barChart'), {
        type: 'bar',
        data: { labels, datasets: [{ data: vals, backgroundColor: '#1f6fb2' }] },
        options: { scales: { y: { beginAtZero: true, max: 5 } }, plugins: { legend: { display: false } } }
    });
};

document.getElementById('downloadCSV').onclick = () => {
    const days = document.getElementById('filterTime').value;
    const url = API_URL + '?action=export_csv' + ((days && days !== 'all') ? '&days=' + encodeURIComponent(days) : '');
    window.location = url;
};

document.getElementById('exportReport').onclick = async () => {
    const { jsPDF } = window.jspdf;
    const doc = new jsPDF();
    doc.setFontSize(16);
    doc.text("Survey Analytics Report", 20, 20);

    const days = document.getElementById('filterTime').value;
    const params = (days && days !== 'all') ? { days } : {};
    const stats = await api('dashboard_stats', 'GET', params);
    const responses = await loadResponses((days && days !== 'all') ? days : null);

    // --- HEADER INFO ---
    doc.setFontSize(12);
    let y = 35;

    doc.text(`Total responses: ${stats.total}`, 20, y);
    y += 10;

    doc.text(`Overall average score: ${stats.overall_avg.toFixed(2)}`, 20, y);
    y += 10;

    // Average Time
    if (stats.average_time) {
        doc.text(`Overall average time: ${stats.average_time.toFixed(2)} seconds`, 20, y);
        y += 15;
    }

    // --- CATEGORY AVERAGES ---
    doc.text("Category averages:", 20, y);
    y += 10;

    (stats.categories || []).forEach(c => {
        doc.text(`${c.title}: ${Number(c.avgv).toFixed(2)}`, 22, y);
        y += 8;
        if (y > 270) { doc.addPage(); y = 20; }
    });

    // --- DAILY SUMMARY ---
    if (responses.length) {
        y += 10;
        doc.text("Summary (Avg & Respondents per Day):", 20, y);
        y += 12;

        const map = {};
        responses.forEach(r => {
            const dayKey = new Date(r.submitted_at).toLocaleDateString();
            if (!map[dayKey]) map[dayKey] = { sum: 0, count: 0 };
            map[dayKey].sum += Number(r.overall) || 0;
            map[dayKey].count++;
        });

        const sortedDays = Object.keys(map).sort((a, b) => new Date(a) - new Date(b));

        sortedDays.forEach(day => {
            const avg = map[day].count ? (map[day].sum / map[day].count).toFixed(2) : 0;
            const line = `${day} — Avg: ${avg} — Respondents: ${map[day].count}`;
            doc.text(line, 22, y);
            y += 8;

            if (y > 270) { doc.addPage(); y = 20; }
        });
    }

    // --- FOOTER ---
    y += 15;
    doc.text("Generated: " + new Date().toLocaleString(), 20, y);

    doc.save('survey_report.pdf');
};


document.getElementById('respFilterTime').onchange = renderResponses;

/* Init */
(async function init() {
    await renderDashboard();
    renderManage();
    renderResponses();
    renderAnalytics();
})();