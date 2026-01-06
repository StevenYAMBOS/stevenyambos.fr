const body = document.body;

const btnTheme = document.querySelector(".fa-moon");
const btnHamburger = document.querySelector(".fa-bars");

const addThemeClass = (bodyClass, btnClass) => {
  body.classList.add(bodyClass);
  btnTheme.classList.add(btnClass);
};

const isDark = () => body.classList.contains("dark");

const updateGitHubWidgetsTheme = (isDarkMode) => {
  const theme = isDarkMode ? "github-dark-blue" : "react";
  const color = isDarkMode ? "90a0d9" : "2978B5";

  const streakImg = document.getElementById("github-streak");
  const overviewImg = document.getElementById("github-overview");
  const langsImg = document.getElementById("github-langs");
  const contribImg = document.getElementById("github-contrib");

  if (streakImg) {
    streakImg.src = `https://github-readme-streak-stats-teal-theta.vercel.app?user=StevenYAMBOS&theme=${theme}`;
  }
  if (overviewImg) {
    overviewImg.src = `https://github-readme-stats-fast.vercel.app/api?username=StevenYAMBOS&show_icons=true&theme=${theme}&hide_title=true`;
  }
  if (langsImg) {
    langsImg.src = `https://github-readme-stats-fast.vercel.app/api/top-langs/?username=StevenYAMBOS&layout=compact&langs_count=8&theme=${theme}`;
  }
  if (contribImg) {
    contribImg.src = `https://ghchart.rshah.org/${color}/StevenYAMBOS`;
  }
};

const getBodyTheme = localStorage.getItem("portfolio-theme") || "light";
const getBtnTheme = localStorage.getItem("portfolio-btn-theme") || "fa-moon";

addThemeClass(getBodyTheme, getBtnTheme);

// Initialize GitHub widgets theme on page load (after DOM is ready)
document.addEventListener("DOMContentLoaded", () => {
  updateGitHubWidgetsTheme(getBodyTheme === "dark");
});

const setTheme = (bodyClass, btnClass) => {
  body.classList.remove(localStorage.getItem("portfolio-theme"));
  btnTheme.classList.remove(localStorage.getItem("portfolio-btn-theme"));

  addThemeClass(bodyClass, btnClass);

  localStorage.setItem("portfolio-theme", bodyClass);
  localStorage.setItem("portfolio-btn-theme", btnClass);

  // Update GitHub widgets theme
  updateGitHubWidgetsTheme(bodyClass === "dark");
};

const toggleTheme = () =>
  isDark() ? setTheme("light", "fa-moon") : setTheme("dark", "fa-sun");

btnTheme.addEventListener("click", toggleTheme);

const displayList = () => {
  const navUl = document.querySelector(".nav__list");

  if (btnHamburger.classList.contains("fa-bars")) {
    btnHamburger.classList.remove("fa-bars");
    btnHamburger.classList.add("fa-times");
    navUl.classList.add("display-nav-list");
  } else {
    btnHamburger.classList.remove("fa-times");
    btnHamburger.classList.add("fa-bars");
    navUl.classList.remove("display-nav-list");
  }
};

btnHamburger.addEventListener("click", displayList);

const scrollUp = () => {
  const btnScrollTop = document.querySelector(".scroll-top");

  if (body.scrollTop > 500 || document.documentElement.scrollTop > 500) {
    btnScrollTop.style.display = "block";
  } else {
    btnScrollTop.style.display = "none";
  }
};

document.addEventListener("scroll", scrollUp);
