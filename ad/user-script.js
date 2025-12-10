/* ===============================
   CONFIG & ELEMENTS
   =============================== */
const apiBase = "api.php";

const welcome = document.getElementById("welcome");
const surveyWrap = document.getElementById("surveyWrap");
const results = document.getElementById("results");
const form = document.getElementById("surveyForm");

const startBtn = document.getElementById("startBtn");
const clearBtn = document.getElementById("clearBtn");
const submitBtn = document.getElementById("submitBtn");
const retakeBtn = document.getElementById("retakeBtn");
const finishBtn = document.getElementById("finishBtn");

let surveyData = [];
let startTime = null;

/* ===============================
   TIME FORMATTER
   =============================== */
function formatTime(seconds) {
    if (!seconds || seconds === 0) return "—";

    const h = Math.floor(seconds / 3600);
    const m = Math.floor((seconds % 3600) / 60);
    const s = Math.floor(seconds % 60);

    if (h > 0) return `${h} hour${h > 1 ? "s" : ""} ${m} min`;
    if (m > 0) return `${m} min ${s} sec`;
    return `${s} second${s !== 1 ? "s" : ""}`;
}

/* ===============================
   PRELOAD SURVEY
   =============================== */
(async function preloadSurvey() {
    try {
        const res = await fetch(`${apiBase}?action=get_survey`);
        const data = await res.json();

        if (data.success && data.categories) {
            surveyData = data.categories;
            updateEstimatedTime(data);
        }
    } catch (err) {
        console.error(err);
    }
})();

/* ===============================
   BUTTON ACTIONS
   =============================== */
startBtn.addEventListener("click", async () => {
    welcome.style.display = "none";
    surveyWrap.style.display = "block";
    startTime = Date.now();
    await loadSurvey();
});

clearBtn.addEventListener("click", () => {
    form.querySelectorAll("input[type=radio]").forEach(r => r.checked = false);
});

retakeBtn.addEventListener("click", () => {
    results.style.display = "none";
    surveyWrap.style.display = "block";
    form.querySelectorAll("input[type=radio]").forEach(r => r.checked = false);
});

finishBtn.addEventListener("click", () => {
    results.style.display = "none";
    welcome.style.display = "block";
});

/* ===============================
   LOAD & RENDER SURVEY
   =============================== */
async function loadSurvey() {
    form.innerHTML = "<p>Loading questions...</p>";
    try {
        const res = await fetch(`${apiBase}?action=get_survey`);
        const data = await res.json();

        if (data.success && data.categories) {
            surveyData = data.categories;
            renderSurvey(surveyData);
            updateEstimatedTime(data);
        } else {
            form.innerHTML = "<p>Failed to load survey questions.</p>";
        }
    } catch (err) {
        console.error(err);
        form.innerHTML = "<p>Error loading survey.</p>";
    }
}

function renderSurvey(categories) {
    form.innerHTML = "";

    const isSmall = window.matchMedia("(max-width: 809px)").matches;
    const labels = isSmall
        ? ["SD", "D", "N", "A", "SA"]
        : ["Strongly Disagree", "Disagree", "Neutral", "Agree", "Strongly Agree"];

    const colors = ["#e74c3c", "#e67e22", "#9ca3af", "#2ecc71", "#27ae60"];
    const sizes = ["size-large", "size-medium", "size-small", "size-medium", "size-large"];

    categories.forEach(cat => {
        form.insertAdjacentHTML("beforeend", `<h3 style="margin-top:20px">${cat.name}</h3>`);

        cat.questions.forEach((q, i) => {
            const qBlock = document.createElement("div");
            qBlock.className = "question-block";

            qBlock.innerHTML = `
                <div class="q-left">
                    <h4>${q.question_text}</h4>
                    <p>${cat.name} — Question ${i + 1}</p>
                </div>
            `;

            const right = document.createElement("div");
            right.className = "q-right";
            const row = document.createElement("div");
            row.className = "radio-row";

            for (let i = 0; i < 5; i++) {
                row.innerHTML += `
                    <label class="radio-btn ${sizes[i]}">
                        <input type="radio" name="q_${q.id}" value="${i + 1}" data-color="${colors[i]}">
                        <div class="radio-label"
                             style="--border-color:${colors[i]};--fill-color:${colors[i]}">
                             ${labels[i]}
                        </div>
                    </label>
                `;
            }

            right.appendChild(row);
            qBlock.appendChild(right);
            form.appendChild(qBlock);
        });
    });
}

/* ===============================
   RESPONSIVE RE-RENDER
   =============================== */
window.addEventListener("resize", () => {
    if (!surveyData.length) return;

    const answers = {};
    form.querySelectorAll("input:checked").forEach(i => answers[i.name] = i.value);

    renderSurvey(surveyData);
    Object.keys(answers).forEach(name => {
        const el = form.querySelector(`input[name="${name}"][value="${answers[name]}"]`);
        if (el) el.checked = true;
    });
});

/* ===============================
   ANSWER STATE HIGHLIGHT
   =============================== */
document.addEventListener("change", e => {
    if (e.target.type === "radio") {
        e.target.closest(".question-block")?.classList.add("answered");
    }
});

/* ===============================
   SUBMIT SURVEY
   =============================== */
submitBtn.addEventListener("click", async () => {
    const blocks = form.querySelectorAll(".question-block");
    if (!blocks.length) return alert("No questions available.");

    for (const block of blocks) {
        const name = block.querySelector("input").name;
        if (!form.querySelector(`input[name="${name}"]:checked`)) {
            return alert("Please answer all questions.");
        }
    }

    const payload = {
        categories: surveyData.map(cat => ({
            id: cat.id,
            questions: cat.questions.map(q => ({
                question_id: q.id,
                rating: +form.querySelector(`input[name="q_${q.id}"]:checked`).value
            }))
        })),
        answer_time: startTime ? Math.floor((Date.now() - startTime) / 1000) : 0
    };

    try {
        const res = await fetch(`${apiBase}?action=submit_survey`, {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(payload)
        });
        const data = await res.json();
        if (data.success) showResults(payload);
        else alert("Submit failed.");
    } catch (err) {
        console.error(err);
        alert("Submission error.");
    }
});

/* ===============================
   SHOW RESULTS
   =============================== */
function showResults(payload) {
    surveyWrap.style.display = "none";
    results.style.display = "block";

    const list = document.getElementById("categoryList");
    list.innerHTML = "";

    const avgs = payload.categories.map(cat => {
        const sum = cat.questions.reduce((a, q) => a + q.rating, 0);
        const avg = (sum / cat.questions.length).toFixed(2);
        list.innerHTML += `<li>${surveyData.find(c => c.id === cat.id).name}: ${avg} / 5</li>`;
        return +avg;
    });

    const overall = (avgs.reduce((a, b) => a + b, 0) / avgs.length).toFixed(2);

    list.innerHTML += `<li style="font-weight:700">Overall Score: ${overall} / 5</li>`;
    document.getElementById("overallScore").textContent = overall;

    let text = "";
    if (overall >= 4.5)
        text = "This reflects an excellent level of satisfaction. Overall, the experience exceeded expectations.";
    else if (overall >= 3.5)
        text = "This indicates a high level of satisfaction, with most aspects meeting user expectations.";
    else if (overall >= 2.5)
        text = "This suggests an average level of satisfaction. Some areas may need improvement.";
    else if (overall >= 1.5)
        text = "This indicates a low level of satisfaction. Several aspects may require attention.";
    else
        text = "This reflects very low satisfaction and highlights the need for significant improvement.";

    document.getElementById("resultText").textContent =
        `Your overall satisfaction score is ${overall}/5. ${text}`;
}

/* ===============================
   ESTIMATED TIME
   =============================== */
function updateEstimatedTime() {
    if (!surveyData || surveyData.length === 0) return;

    // Count total questions
    const totalQuestions = surveyData.reduce((sum, cat) => sum + cat.questions.length, 0);

    // Estimate: 20 seconds per question
    const secondsPerQuestion = 20;
    const totalSeconds = totalQuestions * secondsPerQuestion;
    const minutes = Math.ceil(totalSeconds / 60);

    // Update the div
    const estDiv = document.getElementById("estimatedTime");
    if (estDiv) {
        estDiv.textContent = `Estimated time: ${minutes} minute${minutes > 1 ? 's' : ''}`;
    }
}

/* ===============================
   RADIO COLOR FILL
   =============================== */
document.addEventListener("input", e => {
    if (e.target.type === "radio") {
        e.target.nextElementSibling.style.setProperty(
            "--fill-color",
            e.target.dataset.color
        );
    }
});

/* ===============================
   LIGHT/DARK SWITCH
   =============================== */
const link = document.getElementById("themeStylesheet");
const switchBtn = document.getElementById("switchButton");

// Checks if a "theme" was saved
const savedTheme = localStorage.getItem("theme");
if (savedTheme) {
    link.href = savedTheme;
}

// Toggle theme and save choice
switchBtn.addEventListener("click", function () {
    if (link.href.includes("user.css")) {
        link.href = "userdark.css";
        localStorage.setItem("theme", "userdark.css");
    } else {
        link.href = "user.css";
        localStorage.setItem("theme", "user.css");
    }
});