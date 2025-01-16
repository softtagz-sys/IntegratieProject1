window.addEventListener('load', function() {
    
    const rangeInput = document.querySelector('#rangeInputPlayer1') as HTMLInputElement;
    const selectedValueInput = document.querySelector('#selectedValuePlayer1') as HTMLInputElement;
    
    const rangeInputPlayer2 = document.querySelector('#rangeInputPlayer2') as HTMLInputElement;
    const selectedValueInputPlayer2 = document.querySelector('#selectedValuePlayer2') as HTMLInputElement;
    
    const formPlayers = document.querySelector('#formPlayers') as HTMLFormElement;

    let optionsPlayer1: HTMLInputElement[] = [];
    let optionsPlayer2: HTMLInputElement[] = [];
    
    if (formPlayers) {
        optionsPlayer1 = Array.from(formPlayers.querySelectorAll('.form-check-input')) as HTMLInputElement[];
        optionsPlayer2 = Array.from(formPlayers.querySelectorAll('.form-check-input')) as HTMLInputElement[];
    }    
    let selectedIndex = 0;
    let lastSelectedIndexPlayer1 = 0;
    let lastSelectedIndexPlayer2 = 0;
    let aKeyPressCount = 0;
    let lastAKeyPressTime = 0;
    const debounceTime = 500;
    let player = '';
    let intervalId: NodeJS.Timeout;

    document.addEventListener('keydown', function(event: KeyboardEvent) {
        switch (event?.key) {
            case 's':
            case 'w':
                player = 'player1';
                selectedIndex = lastSelectedIndexPlayer1;
                break;
            case 'ArrowDown':
            case 'ArrowUp':
                player = 'player2';
                selectedIndex = lastSelectedIndexPlayer2;
                break;
        }
    });
    document.addEventListener('keydown', function(event: KeyboardEvent) {
        switch (event?.key) {
            case 'a':
            case 'ArrowLeft':
                (document.getElementById('prevQuestionButton') as HTMLButtonElement)?.click();
                break;
                
            case 'd':
                aKeyPressCount++;
                if (aKeyPressCount === 1) {
                    lastAKeyPressTime = new Date().getTime();
                    intervalId = setInterval(() => {
                        const currentTime = new Date().getTime();
                        if (currentTime - lastAKeyPressTime > debounceTime) {
                            optionsPlayer1[lastSelectedIndexPlayer1].checked = !optionsPlayer1[lastSelectedIndexPlayer1]?.checked;
                            aKeyPressCount = 0;
                            clearInterval(intervalId);
                        }
                    }, debounceTime);
                } else if (aKeyPressCount === 2 && new Date().getTime() - lastAKeyPressTime <= debounceTime) {
                    if (selectedIndex >= 0) {
                        (document.getElementById('nextQuestionButton') as HTMLButtonElement)?.click();
                    }
                    aKeyPressCount = 0;
                    clearInterval(intervalId);
                }
                break;
                
            case 'ArrowRight':
                aKeyPressCount++;
                if (aKeyPressCount === 1) {
                    lastAKeyPressTime = new Date().getTime();
                    intervalId = setInterval(() => {
                        const currentTime = new Date().getTime();
                        if (currentTime - lastAKeyPressTime > debounceTime) {
                            optionsPlayer2[lastSelectedIndexPlayer2].checked = !optionsPlayer2[lastSelectedIndexPlayer2]?.checked;
                            aKeyPressCount = 0;
                            clearInterval(intervalId);
                        }
                    }, debounceTime);
                } else if (aKeyPressCount === 2 && new Date().getTime() - lastAKeyPressTime <= debounceTime) {
                    if (selectedIndex >= 0) {
                        (document.getElementById('nextQuestionButton') as HTMLButtonElement)?.click();
                    }
                    aKeyPressCount = 0;
                    clearInterval(intervalId);
                }
                break;
            case 'w':
                if (selectedIndex > 0) {
                        optionsPlayer1[selectedIndex]?.parentElement?.classList.remove('selected-option-player1');
                        selectedIndex--;
                        optionsPlayer1[selectedIndex]?.parentElement?.classList.add('selected-option-player1');
                        lastSelectedIndexPlayer1 = selectedIndex;
                    
                }
                if (rangeInput && rangeInput.valueAsNumber < parseInt(rangeInput.max || '0')) {
                    rangeInput.valueAsNumber++;
                    selectedValueInput.value = rangeInput?.value;
                }
                break;
            case 'ArrowUp':
                if (selectedIndex > 0) {
                        optionsPlayer2[selectedIndex]?.parentElement?.classList.remove('selected-option-player2');
                        selectedIndex--;
                        optionsPlayer2[selectedIndex]?.parentElement?.classList.add('selected-option-player2');
                        lastSelectedIndexPlayer2 = selectedIndex;
                }
                if (rangeInputPlayer2 && rangeInputPlayer2.valueAsNumber < parseInt(rangeInputPlayer2.max || '0')) {
                    rangeInputPlayer2.valueAsNumber++;
                    selectedValueInputPlayer2.value = rangeInputPlayer2?.value;
                }
                break;
            case 's':
            case 'ArrowDown':
                if (selectedIndex >= 0 && selectedIndex < optionsPlayer1.length - 1) {
                    if (player === 'player1') {
                        optionsPlayer1[selectedIndex]?.parentElement?.classList.remove('selected-option-player1');
                        selectedIndex++;
                        optionsPlayer1[selectedIndex]?.parentElement?.classList.add('selected-option-player1');
                        lastSelectedIndexPlayer1 = selectedIndex;
                    } else if (player === 'player2') {
                        optionsPlayer2[selectedIndex]?.parentElement?.classList.remove('selected-option-player2');
                        selectedIndex++;
                        optionsPlayer2[selectedIndex]?.parentElement?.classList.add('selected-option-player2');
                        lastSelectedIndexPlayer2 = selectedIndex;
                    }
                }
                if (player === 'player1' && rangeInput && rangeInput.valueAsNumber > parseInt(rangeInput.min || '0')) {
                    rangeInput.valueAsNumber--;
                    selectedValueInput.value = rangeInput?.value;
                } else if (player === 'player2' && rangeInputPlayer2 && rangeInputPlayer2.valueAsNumber > parseInt(rangeInputPlayer2.min || '0')) {
                    rangeInputPlayer2.valueAsNumber--;
                    selectedValueInputPlayer2.value = rangeInputPlayer2?.value;
                }
                break;
        }
    });
});