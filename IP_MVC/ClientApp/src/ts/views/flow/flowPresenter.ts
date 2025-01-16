import * as client from "./restFlowClient"
import {Flow} from "../../models/Flows.interfaces";
import {updateSwiper} from "./createScroll";

const createButton = document.querySelectorAll('#createFLowButton');

export function setupEditEventListener() {
    const editButtons = document.querySelectorAll('.edit-flow-btn');
    const backButton = document.querySelectorAll('.btn-back');


    editButtons.forEach(button => {
        button.addEventListener('click', () =>
            editFlow(button)
        )
    });
    backButton.forEach(button => {
        button.addEventListener('click', () => {
            const cardContainer = button.closest('.flip-card') as HTMLBodyElement;
            if (cardContainer) {
                const cardInner = cardContainer.querySelector('.flip-card-inner');
                if (cardInner) {
                    cardInner.classList.toggle('flipped');
                }
            }
        })
    });

    createButton.forEach(button => {
        button.addEventListener('click', async () => {
            addFlow().then(() => {
                const nameInput = document.getElementById('NewNameInput') as HTMLInputElement;
                const descriptionInput = document.getElementById('NewDescriptionInput') as HTMLInputElement;
                nameInput.value = '';
                descriptionInput.value = '';
            });
        })
    });

    deleteFlow();
}

async function showUpdatedFlow(id: string, projectCard: HTMLElement) {
    console.log('showFlow', id, projectCard);
    try {
        const flow = await client.getFlow(id);
        const cardBody = projectCard.querySelector('.front')!;

        cardBody.innerHTML = `
                            <h5 class="card-title">${flow.name}</h5>
                            <p class="card-text">${flow.description}</p>`;
    } catch (e) {
        console.error('Error showing flow: ', e);
    }

}

async function changeFlow(flowCard: HTMLElement) {
    const nameInput = flowCard.querySelector('#nameInput') as HTMLInputElement;
    const descriptionInput = flowCard.querySelector('#descriptionInput') as HTMLInputElement;
    const flowIdInput = flowCard.querySelector('#flowId') as HTMLInputElement;

    console.log(nameInput.value, descriptionInput.value, flowIdInput.value);
    try {
        await client.updateFlow(nameInput.value, descriptionInput.value, flowIdInput.value);
    } catch (error) {
        console.error('Error updating flow:', error);
        alert('There was an issue updating the flow. Please try again.');
    }

    await showUpdatedFlow(flowIdInput.value, flowCard);

    const cardInner = flowCard.querySelector('.flip-card-inner');
    if (cardInner) {
        cardInner.classList.toggle('flipped');
    }
}

async function addFlow() {
    const nameInput = document.getElementById('NewNameInput') as HTMLInputElement;
    const descriptionInput = document.getElementById('NewDescriptionInput') as HTMLInputElement;
    const startDateInput = document.getElementById('NewStartDateInput') as HTMLInputElement;
    const endDateInput = document.getElementById('NewEndDateInput') as HTMLInputElement;
    const projectIdInput = document.getElementById('projectIdInput') as HTMLInputElement;
    const parentFlowIdInput = document.getElementById('parentFlowIdInput') as HTMLInputElement;

    if (!nameInput.value || !startDateInput.value || !endDateInput.value) {
        alert('Please fill in all required fields name, start date, end date.');
        return;
    }
    
    if (startDateInput.value > endDateInput.value) {
        alert('Start date cannot be after end date.');
        return;
    }
    
    const flow: Flow = {
        NewName: nameInput.value,
        NewDescription: descriptionInput.value,
        NewStartDate: new Date(startDateInput.value),
        NewEndDate: new Date(endDateInput.value),
        NewProjectId: parseInt(projectIdInput.value),
        NewParentFlowId: parentFlowIdInput.value ? parseInt(parentFlowIdInput.value) : null,
        IsParentFlow: !parentFlowIdInput.value
    };

    try {
        await client.createFlow(flow);
        closePopup();
    } catch (error) {
        console.error('Error creating flow:', error);
        alert('There was an issue creating the flow. Please try again.');
    }
}

function editFlow(editButton: Element) {
    const cardContainer = editButton.closest('.flip-card') as HTMLBodyElement;
    if (cardContainer) {
        const cardInner = cardContainer.querySelector('.flip-card-inner');
        if (cardInner) {
            cardInner.classList.toggle('flipped');

            const updateFlowButton = cardContainer.querySelector("#updateFlowButton");
            if (updateFlowButton) {
                updateFlowButton.addEventListener('click', async () => changeFlow(cardContainer), {once: true});
            } else {
                console.error("updateFlowButton not found");
            }
        }
    }
}

export function appendFlowToPage(flow: Flow, flowId: number) {
    const flowDataElement = document.getElementById('row');
    if (!flowDataElement) return;

    const isAdminRole = document.getElementById('userRole') as HTMLInputElement;
    const isActiveProject = document.getElementById('activeProject') as HTMLInputElement;
    let deleteButtonHtml = '';
    let editButtonHtml = '';
    if (isAdminRole && isAdminRole.value === "True" && isActiveProject && isActiveProject.value === "False") {
        deleteButtonHtml = `<button class="btn btn-blue bi bi-trash deleteFlowButton create-btn" data-flow-id="${flowId}" href="/Flow/Delete?flowId=${flowId}"></button>`;
        editButtonHtml = `<button data-edit-flow data-flow-id="${flowId}" class="btn btn-blue py-0 edit-flow-btn">Edit</button>`;
    }

    /*const flowActionsHtml = `
        <a href="/Flow/SubFlow?parentFlowId=${flow.NewParentFlowId}&active=${isActiveProject}" class="btn btn-blue">Go to Subflows</a>
    `;
*/
    const subFlowEditButtonHtml = `       
            <div class="btn-group">
                <button class="btn bi bi-menu-button-wide btn-blue dropdown-toggle py-0" type="button" data-bs-toggle="dropdown" aria-expanded="false">
                </button>
                <ul class="dropdown-menu dropdown-menu-right background-color-light-blue">
                    <li class="px-1 pb-1">
                        <a class="btn btn-blue dropdown-item p-2 d-flex justify-content-center" href="/Flow/Edit/${flowId}" class="btn btn-primary">Edit Questions</a>
                    </li>
                    <li class="px-1">
                        <a class="btn btn-blue dropdown-item p-2 d-flex justify-content-center" href="/Flow/SubFlow/${flowId}" class="btn btn-primary">Edit SubFlows</a>
                    </li>
                </ul>
            </div>  
    `;

    flowDataElement.innerHTML += `
        <div class="col col-sm-12 col-md-6 col-lg-4 col-xl-4 pb-3">
            <div class="flip-card bg-transparent">
                <div class="flip-card-inner">
                    <div class="flip-card-front w-100">
                        <div class="small-card card border-1 border-black position-absolute card-clickable w-100" data-parent-flow-id="${flow.NewParentFlowId}">
                            <div class="front card-body overflow-y-scroll overflow-x-hidden">
                                <h5 class="card-title">${flow.NewName}</h5>
                                <p class="card-text">${flow.NewDescription}</p>
                            </div>
                            <div class="d-flex justify-content-between align-items-center position-sticky button-container overflow-x-scroll overflow-y-hidden px-3">
                                <div>${deleteButtonHtml}</div>
                                <div>${editButtonHtml}</div>
<!--                                <div>$flowActionsHtml}</div>-->
                            </div>
                        </div>
                    </div>
                    <div class="flip-card-back w-100 position-relative">
                        <div class="small-card card border-1 border-black card-clickable" data-parent-flow-id="${flowId}">
                            <div class="card-body overflow-y-scroll overflow-x-hidden">
                                <input type="hidden" id="flowId" value="${flowId}"/>
                                <div class="form-group">
                                    <label for="nameInput">Name</label>
                                    <input type="text" class="form-control" id="nameInput" value="${flow.NewName}" required>
                                </div>
                                <div class="form-group">
                                    <label for="descriptionInput">Description</label>
                                    <input type="text" class="form-control" id="descriptionInput" value="${flow.NewDescription}" required>
                                </div>
                            </div>
                            <div class="d-flex justify-content-between align-items-center position-sticky button-container py-1">
                                <div><button class="btn btn-blue py-0 btn-back">Back</button></div>
                                <div><button type="submit" class="btn btn-blue py-0" id="updateFlowButton">Update</button></div>
                                <div>${subFlowEditButtonHtml}</div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    `;
}

export function appendSubFlowToPage(flow: Flow, subFlowId: number) {
    const flowDataElement = document.getElementById('swiper-element');
    const isAdminRole = document.getElementById('userRole') as HTMLInputElement;
    const isActiveProject = document.getElementById('activeProject') as HTMLInputElement;
    if (!flowDataElement) return;
    const randomIndex = Math.floor(Math.random() * 4) + 1;
    const imageUrl = `https://storage.googleapis.com/phygital-public/Flows/flow_page_hands_${randomIndex}.png`;

    let deleteButtonHtml = '';
    let editButtonHtml = '';

    if (isAdminRole && isAdminRole.value === "True" && isActiveProject && isActiveProject.value === "False") {
        deleteButtonHtml = `<button class="btn btn-white bi bi-trash deleteFlowButton create-btn" data-flow-id="${subFlowId}" href="/Flow/Delete?flowId=${subFlowId}"></button>`;
        editButtonHtml = ` <a class="btn btn-white py-0" href="/Flow/Edit/${subFlowId}" class="btn btn-primary">Edit Questions</a>
`;
    }

    //TODO: Add START FLOW BUTTON
    const flowActionsHtml = `
        <a class="btn btn-white">Start Flow</a>
    `;

    flowDataElement.innerHTML += `
        <div class="swiper-slide">
            <div class="slide-card">
                <img src="${imageUrl}" class="card-img-top w-100 vh-100 z-1 position-relative" alt="Afbeelding_van_flow">
                <div class="card border-1 border-black h-50 position-absolute card-clickable">
                    <div class="align-items-center h-100">
                        <div class="card-body ">
                            <h5 class="card-title">${flow.NewName}</h5>
                            <p class="card-text">${flow.NewDescription}</p>
                        </div>
                    </div>
                      <div class="d-flex text-center position-sticky py-2 button-container">
                          <div class="flex-grow-1">${deleteButtonHtml}</div>
                          <div class="flex-grow-1">${editButtonHtml}</div>
                          <div class="flex-grow-1">${flowActionsHtml}</div>
                      </div>
                </div>
            </div>
        </div>
    `;

    updateSwiper();
}

async function deleteFlow() {
    const deleteButtonsForFlow = document.getElementsByClassName('deleteFlowButton') as HTMLCollectionOf<HTMLButtonElement>
    if (deleteButtonsForFlow === null) return;
    for (let i = 0; i < deleteButtonsForFlow.length; i++) {
        deleteButtonsForFlow[i].addEventListener('click', async (event) => {
            event.preventDefault();
            const flowId = deleteButtonsForFlow[i].getAttribute('data-flow-id') as string;
            const subFlows = await client.getSubFlows(flowId);
            const subFlowsCount = subFlows.length;
            if (subFlowsCount > 0) {
                if (confirm(`This flow contains ${subFlowsCount} subflows, are you sure you want to delete this flow?`)) {
                    window.location.href = deleteButtonsForFlow[i].getAttribute('href') as string;
                }
            } else {
                if (confirm('Are you sure you want to delete this flow?')) {
                    window.location.href = deleteButtonsForFlow[i].getAttribute('href') as string;
                }
            }
        });
    }
}

function closePopup() {
    const popupOverlay = document.getElementById('popupOverlay');
    if (popupOverlay != null) {
        popupOverlay.style.display = 'none';
    }
}

setupEditEventListener();