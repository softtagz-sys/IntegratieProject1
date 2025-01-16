import * as client from "./restQuestionClient"
import {Question} from "../../models/Questions.interfaces";

const titleText = document.getElementById('titleText')!;

const questionId = document.getElementById('questionId') as HTMLInputElement;
const questionType = document.getElementById('questionType') as HTMLInputElement;
const titleInput = document.getElementById('titleInput') as HTMLInputElement;
const mediaInput = document.getElementById('mediaUpload') as HTMLInputElement;

const changeNameButton = document.getElementById('editButton')!;
const updateNameButton = document.getElementById('updateButton')!;
const uploadMediaButton = document.getElementById('uploadButton')!;
const addConditionalQuestionsButton = document.getElementById('addConditionalQuestionsButton')!;
const saveQuestionButton = document.getElementById('saveQuestionButton') as HTMLButtonElement;


if (questionType.value == "MultipleChoice" || questionType.value == "SingleChoice") {
    const newOption = document.getElementById('newOption') as HTMLInputElement;
    const addOptionButton = document.getElementById('buttonToAdd')!;
    const optionTable = document.getElementById('optionTable')!;

    // Loads all options from question and shows them in table
    async function showOptions() {
        try {
            const options = await client.getOptions(questionId.value);
            if (options==null || options.length == 0) {
                optionTable.innerHTML = '<p>This question does not have any options yet</p>'
                return;
            }
            optionTable.innerHTML = options.reduce(
                (acc: string, option: any) => `${acc}
                <tr>
                <td>${option.text}</td>
                <td>
                    <button delete-option option-id="${option.id}" type="button" class="btn btn-blue">Delete option</button>
                </td>
                <td>
                    <button class="select-question" style="display: none" option-id="${option.id}" type="button" class="btn btn-blue">Select folowup question</button>
                </td>
                </tr>`, "<table>"
            ) + "</table>"
            console.clear();
        } catch (e) {
            optionTable.innerHTML = '<p>This question does not have any options yet</p>'
            console.error("Error showing options in table: ", e);
        }
    }

    async function removeOption(element: HTMLElement) {
        if (element.hasAttribute('delete-option')) {
            let option = element.getAttribute("option-id");
            if (option != null) {
                try {
                    await client.deleteOption(option);
                } catch (e) {
                    console.error(`ERROR DELETING OPTION ${option}`, e);
                }
            }
            await showOptions();
        }
    }

    async function addOption(option: string) {
        try {
            const alphanumericRegex = /[a-zA-Z0-9]/;
            if (!alphanumericRegex.test(option)) {
                console.error("Option must contain at least one letter or number.");
                return;
            }
            
            const options = await client.getOptions(questionId.value)
            const alreadyExists = options.some((getOption: {text: string}) => getOption.text == option)
            if (alreadyExists) {
                console.error("Option already exists.");
                return
            }
            
            await client.postOption(questionId.value, option)
        } catch (e) {
            console.error("Error adding option: ", e);
        }
        await showOptions();
        newOption.value = '';
    }

    showOptions();

    document.addEventListener('click', event => removeOption(event.target as HTMLElement));
    addOptionButton.addEventListener('click', event => {
        event.preventDefault();
        addOption(newOption.value);
    });


} else if (questionType.value == "Range") {
    const minInput = document.getElementById('min') as HTMLInputElement;
    const maxInput = document.getElementById('max') as HTMLInputElement;
    const updateRangeButton = document.getElementById('updateRangeValues')!;

    updateRangeButton.addEventListener('click', event => {
            event.preventDefault();
            client.updateQuestionRange(questionId.value, minInput.value, maxInput.value);
        }
    );
}

async function updateMedia() {

    //TODO: Error handling als geen file wordt geupload

    const file = mediaInput.files?.[0];

    if (file) {
        const formData = new FormData();
        formData.append('file', file);
        formData.append('questionId', questionId.value);

        await client.postMedia(formData);
    }
}

// display the input element to change question name
function displayNameChange(display: boolean) {
    if (display) {
        titleText.classList.add('d-none');
        changeNameButton.classList.add('d-none');
        titleInput.classList.remove('d-none');
        updateNameButton.classList.remove('d-none');
    } else {
        titleText.classList.remove('d-none');
        changeNameButton.classList.remove('d-none');
        titleInput.classList.add('d-none');
        updateNameButton.classList.add('d-none');
    }
}

async function showSeperateAddButtons() {
    const selectQuestionsButtons = document.querySelectorAll('.select-question') as NodeListOf<HTMLButtonElement>;
    const flowId = document.getElementById('flowId') as HTMLInputElement;
    const position = document.getElementById('position') as HTMLInputElement;

    for (const selectButton of selectQuestionsButtons) {
        if (selectButton == null) return;
        selectButton.style.display = 'block';


        let questionDropdown = selectButton.parentElement?.querySelector('select');
        if (!questionDropdown) {
            questionDropdown = document.createElement('select');
            questionDropdown.style.display = 'none';
            selectButton.parentElement?.appendChild(questionDropdown);
        }

        const optionId = selectButton.getAttribute('option-id') as string;
        if (optionId == null) return;
        
       
        
        try {
            selectButton.addEventListener('click', async (event) => {
                event.preventDefault();
                const currentOption = await client.getOptionById(optionId);
                const currentNextQuestionId = currentOption.nextQuestionId;

                document.querySelectorAll('select').forEach(select => {
                    select.innerHTML = '';
                    select.style.display = 'none';
                });
                
                questionDropdown.innerHTML = (await client.getQuestionsByFlowIdAfterPosition(flowId.value, position.value)).reduce(
                    (acc: string, question: Question) => `${acc}
                <option id="${question.id}" value="${question.text}" ${question.id == currentNextQuestionId ? 'selected': ''}>${question.text}</option>`, ""
                );
                questionDropdown.innerHTML+= `<option id="-1" value="End flow" ${currentNextQuestionId == -1 ? 'selected': ''}>End flow</option>`;
                questionDropdown.style.display = 'block';
                
                saveQuestionButton.style.display = 'block';
            });
            
        } catch(e) {
            questionDropdown.innerHTML = '<p>This question does not have any follow-up questions yet</p>'
            console.error("Error showing options in table: ", e);
        }
    }
}

saveQuestionButton?.addEventListener('click', event => {
    event.preventDefault();
    const selectedQuestions = document.querySelectorAll('select') as NodeListOf<HTMLSelectElement>;
    const visibleSelectFieldIndex = Array.from(selectedQuestions).findIndex(select => select.style.display == 'block');
    const selectedRedirectedQuestion = selectedQuestions[visibleSelectFieldIndex]
    const button     = document.querySelectorAll('.select-question') as NodeListOf<HTMLButtonElement>;
    const optionId = button[visibleSelectFieldIndex].getAttribute('option-id') as string;
    const selectedQuestionId = selectedRedirectedQuestion.options[selectedRedirectedQuestion.selectedIndex].id;
    
    if (optionId == null || questionId.value == null || selectedQuestionId == null) 
        return;
    client.ChangeRedirectedIdFromOption(questionId.value, optionId, selectedQuestionId);
});

changeNameButton.addEventListener('click', event => displayNameChange(true));
updateNameButton.addEventListener('click', event =>
    client.updateQuestionTitle(questionId.value, titleInput.value)
        .then(() => displayNameChange(false))
);
uploadMediaButton.addEventListener('click', event => updateMedia());

addConditionalQuestionsButton?.addEventListener('click', event => {
    showSeperateAddButtons();
});