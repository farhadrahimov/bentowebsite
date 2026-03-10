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

  // Lightbox with navigation, keyboard, and swipe
  const lb = document.getElementById("lb");
  if (lb) {
    const lbImg = lb.querySelector("[data-lb-img]");
    const lbCounter = lb.querySelector("[data-lb-counter]");
    const lbPrev = lb.querySelector("[data-lb-prev]");
    const lbNext = lb.querySelector("[data-lb-next]");
    let items = [];
    let current = 0;
    let touchStartX = 0;

    function getItems() {
      return Array.from(document.querySelectorAll("[data-lightbox-src]"));
    }

    function setImg(src) {
      lbImg.classList.add("fading");
      setTimeout(() => {
        lbImg.src = src;
        lbImg.classList.remove("fading");
      }, 160);
    }

    function updateNav() {
      const multi = items.length > 1;
      lbPrev.classList.toggle("hidden", !multi);
      lbNext.classList.toggle("hidden", !multi);
      if (lbCounter) {
        lbCounter.textContent = multi ? `${current + 1} / ${items.length}` : "";
      }
    }

    function open(idx) {
      items = getItems();
      current = Math.max(0, Math.min(idx, items.length - 1));
      setImg(items[current].getAttribute("data-lightbox-src"));
      updateNav();
      lb.classList.add("open");
      document.body.style.overflow = "hidden";
    }

    function close() {
      lb.classList.remove("open");
      document.body.style.overflow = "";
      lbImg.src = "";
    }

    function prev() {
      if (items.length < 2) return;
      current = (current - 1 + items.length) % items.length;
      setImg(items[current].getAttribute("data-lightbox-src"));
      updateNav();
    }

    function next() {
      if (items.length < 2) return;
      current = (current + 1) % items.length;
      setImg(items[current].getAttribute("data-lightbox-src"));
      updateNav();
    }

    // Click
    document.addEventListener("click", (e) => {
      const trigger = e.target.closest("[data-lightbox-src]");
      if (trigger) {
        const all = getItems();
        open(all.indexOf(trigger));
        return;
      }
      if (e.target.closest("[data-lb-close]") || e.target === lb) { close(); return; }
      if (e.target.closest("[data-lb-prev]")) { prev(); return; }
      if (e.target.closest("[data-lb-next]")) { next(); return; }
    });

    // Keyboard
    document.addEventListener("keydown", (e) => {
      if (!lb.classList.contains("open")) return;
      if (e.key === "Escape") close();
      else if (e.key === "ArrowLeft") prev();
      else if (e.key === "ArrowRight") next();
    });

    // Touch / swipe
    lb.addEventListener("touchstart", (e) => {
      touchStartX = e.touches[0].clientX;
    }, { passive: true });
    lb.addEventListener("touchend", (e) => {
      const dx = e.changedTouches[0].clientX - touchStartX;
      if (Math.abs(dx) > 48) { dx < 0 ? next() : prev(); }
    }, { passive: true });
  }
})();

