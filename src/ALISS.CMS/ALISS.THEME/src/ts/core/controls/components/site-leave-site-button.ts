
class LeaveSite {
    constructor() {
        enum CLASSES {
            button = 'aliss-leave__button',
        }

        const google = "https://www.google.com/";

        const stateListenerAdd = (button: HTMLButtonElement) => {
            button.addEventListener('click', (e) => {
                e.preventDefault();
                localStorage.clear();
                openUrls();
            });
            document.addEventListener('keydown', (e) => {
                if (e.which === 27) {
                    localStorage.clear();
                    openUrls();
                }
            });
        };

        const openUrls = () => {
            let win = window.open(google, '_blank');
            win?.focus();
            window.location.replace(google);

            // setTimeout(() => {
            //     window.history.pushState(null, '', window.location.href);
            //     window.onpopstate = function () {
            //         window.history.go(1);
            //     };
            // }, 1000);
        }

        const init = () => {
            let leaveSiteButton = document.querySelector("." + CLASSES.button) as HTMLButtonElement;
            if (typeof (leaveSiteButton) != 'undefined' && leaveSiteButton != null) {
                stateListenerAdd(leaveSiteButton);
            }
        };

        init();
    }
}

export default LeaveSite;
