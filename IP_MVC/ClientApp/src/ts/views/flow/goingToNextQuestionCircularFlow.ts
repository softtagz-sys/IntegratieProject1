let countdown: number = 30;
let isPaused: boolean = false;
const countdownElement: HTMLElement | null = document.getElementById('countdown');
const popupButton: HTMLElement | null = document.getElementById('popupButton');
const closePopupButton: HTMLElement | null = document.getElementById('closePopup');
const popupOverlay = document.getElementById('popupOverlay')!;
const submitButton = document.getElementById('createButton')!;

let timeoutId: number | null = null;

export function updateCountdown(): void {
    if (countdownElement) {
        countdownElement.innerText = countdown.toString();
        if (!isPaused) {
            countdown--;

            if (countdown < 0) {
                const myForm: HTMLFormElement | null = document.getElementById('myForm') as HTMLFormElement;
                if (myForm) {
                    myForm.setAttribute('action', '/Flow/SaveAnswerAndRedirect');
                    const redirectedQuestionIdInput = document.getElementById("redirectedQuestionId") as HTMLInputElement;
                    if (redirectedQuestionIdInput) {
                        redirectedQuestionIdInput.value = (parseInt(redirectedQuestionIdInput.value) + 1).toString();
                    }
                    myForm.submit();
                }
            } else {
                timeoutId = window.setTimeout(updateCountdown, 1000);
            }
        }
    }
}

export function showPopup(open: boolean) {
    if (open) {
        popupOverlay.style.display = 'block';
        pauseCountdown();
    } else {
        popupOverlay.style.display = 'none';
        resumeCountdown();
    }
}

submitButton.addEventListener('click', () => {
    showPopup(false);
    // Ensure that the timer is not running before restarting it
    if (timeoutId === null) {
        updateCountdown();
    }
});

popupOverlay.addEventListener('click', function (event) {
    if (event.target === popupOverlay) {
        showPopup(false);
    }
});

function pauseCountdown(): void {
    isPaused = true;
    if (timeoutId !== null) {
        clearTimeout(timeoutId);
        timeoutId = null;
    }
}

function resumeCountdown(): void {
    isPaused = false;
    if (timeoutId === null) {
        updateCountdown();
    }
}

if (popupButton) {
    popupButton.addEventListener('click', () => {
        showPopup(true);
    });
}

if (closePopupButton) {
    closePopupButton.addEventListener('click', () => {
        showPopup(false);
    });
}

updateCountdown();