import * as client from "./restQuestionClient";
import {Question} from "../../models/Questions.interfaces";

const createQuestionButton = document.getElementById('createButton')!;
const saveAndRedirectButton = document.getElementById('saveAndRedirectButton')!;

export function setupEditEventListener() {
    createQuestionButton.addEventListener('click', async () => {
        addQuestion().then(() => {
            const textInput = document.getElementById('newQuestionText') as HTMLInputElement;
            const typeInput = document.getElementById('newQuestionType') as HTMLInputElement;
            textInput.value = '';
            typeInput.value = '';
        });
    });

    saveAndRedirectButton.addEventListener('click', async () => {
        const form = document.getElementById('flowEditForm') as HTMLFormElement;
        const endDate = document.getElementById('NewEndDate') as HTMLInputElement;
        const startDate = document.getElementById('NewStartDate') as HTMLInputElement;
        const name = document.getElementById('NewName') as HTMLInputElement;

        if (!endDate.value || !startDate.value || !name.value) {
            alert('Please fill in all required fields name, start date, end date.');
            return;
        }

        if (startDate.value > endDate.value) {
            alert('Start date cannot be after end date.');
            return;
        }
        
        if (form) {
            form.submit();
        }
    });
    deleteQuestion();
}

async function addQuestion() {
    const textInput = document.getElementById('newQuestionText') as HTMLInputElement;
    const typeInput = document.getElementById('newQuestionType') as HTMLSelectElement;
    const flowIdInput = document.getElementById('flowIdInput') as HTMLInputElement;
    try {
        const question = await client.createQuestion(textInput.value, typeInput.value, flowIdInput.value);
        if (question != null) {
            showQuestion(question);
        }
    } catch (error) {
        console.error('Error creating flow:', error);
        alert('There was an issue creating the flow. Please try again.');
    }
}

export function showQuestion(question: Question) {

    const questionsContent = document.getElementById('questions') as HTMLTableElement;
    let deleteButtonHtml = '';
    let editButtonHtml = '';

    deleteButtonHtml = `<button class="btn btn-blue deleteQuestionButton bi bi-trash" data-question-id="${question.id}"  href="/Question/Delete?questionId=${question.id}"></button>`;
    editButtonHtml = ` <a class="btn btn-blue py-0" href="/Question/Edit?questionId=${question.id}" class="btn btn-primary">Edit</a>`;

    questionsContent.innerHTML += `


        <tbody>
        <tr>
            <td class="question" data-question-id="@{question.Text}" data-position="@question.Position">${question.text}</td>
            <td class="align-content-center">
                <div class="row">
                    <div class="col-6 ">
                        <div>${editButtonHtml}</div>
                    </div>
                    <div class="col-6">
                        <div>${deleteButtonHtml}</div>
                    </div>
            </div>
        </td>
        </tr>
        </tbody>
    `;
}

async function deleteQuestion() {
    const deleteButtonsForQuestion = document.getElementsByClassName('deleteQuestionButton') as HTMLCollectionOf<HTMLButtonElement>
    if (deleteButtonsForQuestion === null) return;

    for (let i = 0; i < deleteButtonsForQuestion.length; i++) {
        deleteButtonsForQuestion[i].addEventListener('click', async (event) => {
            event.preventDefault();

            if (confirm('Are you sure you want to delete this flow?')) {
                window.location.href = deleteButtonsForQuestion[i].getAttribute('href') as string;
            }

        });
    }
}

setupEditEventListener();