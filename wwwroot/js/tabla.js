const initTabla = (config) => {
  const { endpoint, containerId, extraParamsFn } = config;

  const init = (page = 1) => {
    const baseParams = { page };
    const extraParams = extraParamsFn ? extraParamsFn() : {};
    const params = new URLSearchParams({ ...baseParams, ...extraParams });

    fetch(`${endpoint}?${params.toString()}`)
      .then((res) => res.text())
      .then((html) => {
        document.getElementById(containerId).innerHTML = html;
        attachEvents();
      });
  };

  const attachEvents = () => {
    document
      .querySelectorAll(`#${containerId} .page-link[data-page]`)
      .forEach((link) => {
        link.addEventListener("click", (e) => {
          e.preventDefault();
          const page = link.getAttribute("data-page");
          init(page);
        });
      });
  };

  return {
    init,
    attachEvents,
  };
};
