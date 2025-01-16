window.onload = function() {
    let playerToggles = (document.getElementsByClassName('classPlayerToggle'));

    for (let i = 0; i < playerToggles.length; i++) {
        let playerToggle = playerToggles[i] as HTMLInputElement;
        
        playerToggle.addEventListener('click', function(event) {
            let target = event.target as HTMLInputElement;
            // let playerCount = (playerToggles[i] as HTMLInputElement).checked ? 2 : 1;
            let playerCount = target.checked ? 2 : 1;
            updatePlayerCount(playerCount);
        });
        
        updatePlayerCount(1);
    }
};

function updatePlayerCount(playerCount: number) {
    console.log('Player count before fetch: ' + playerCount);
    
    fetch('/api/Flows/SetPlayerCount', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({ playerCount: playerCount })
    })
        .catch((error) => {
            console.error('Error:', error);
        });
}