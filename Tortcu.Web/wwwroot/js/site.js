(() => {
  const root = document.documentElement;
  const storageKey = "tortcu-theme";

  function applyTheme(theme) {
    if (!theme) {
      root.removeAttribute("data-theme");
      return;
    }
    root.setAttribute("data-theme", theme);
  }

  function getPreferredTheme() {
    const saved = localStorage.getItem(storageKey);
    if (saved === "dark" || saved === "light") return saved;
    // Default: always light until user dəyişir
    return "light";
  }

  document.addEventListener("click", (e) => {
    const btn = e.target.closest("[data-theme-toggle]");
    if (!btn) return;

    const current = root.getAttribute("data-theme") || getPreferredTheme();
    const next = current === "dark" ? "light" : "dark";
    localStorage.setItem(storageKey, next);
    applyTheme(next);
  });

  // init theme (default: light)
  applyTheme(getPreferredTheme());

  // staggered card reveal on scroll
  const observer = new IntersectionObserver(
    (entries) => {
      entries.forEach((entry) => {
        if (entry.isIntersecting) {
          const delay = entry.target.dataset.delay || 0;
          setTimeout(() => entry.target.classList.add("is-visible"), delay);
          observer.unobserve(entry.target);
        }
      });
    },
    { threshold: 0.1 }
  );

  document.querySelectorAll(".card").forEach((card, i) => {
    const parent = card.parentElement;
    const siblings = parent ? Array.from(parent.querySelectorAll(":scope > .card")) : [card];
    const index = siblings.indexOf(card);
    card.dataset.delay = index * 80;
    observer.observe(card);
  });

  // simple gallery lightbox
  const modal = document.querySelector("[data-lightbox-modal]");
  if (modal) {
    const img = modal.querySelector("img");
    document.addEventListener("click", (e) => {
      const item = e.target.closest("[data-lightbox-src]");
      if (item && img) {
        const src = item.getAttribute("data-lightbox-src");
        if (src) img.setAttribute("src", src);
        modal.classList.add("open");
        return;
      }

      if (e.target.closest("[data-lightbox-close]") || e.target === modal) {
        modal.classList.remove("open");
        if (img) img.setAttribute("src", "");
      }
    });

    document.addEventListener("keydown", (e) => {
      if (e.key === "Escape") {
        modal.classList.remove("open");
        if (img) img.setAttribute("src", "");
      }
    });
  }
})();

