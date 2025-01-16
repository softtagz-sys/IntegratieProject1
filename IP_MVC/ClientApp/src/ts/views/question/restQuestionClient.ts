// import {showQuestion} from "./createQuestionPresenter";
import {Question} from "../../models/Questions.interfaces";

export async function getOptions(questionId: string) {
    const response = await fetch(`/api/Options/GetAllOptions/${questionId}`);
    if (!response.ok) {
        throw Error(`Unable to get options: ${response.status} ${response.statusText}`);
    }
    return await response.json();
}

export async function updateQuestionTitle(questionId: string, title: string) {
    try {
        const response = await fetch(`/api/Questions/${questionId}/Title`, {
            method: 'PUT',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(title)
        });
        if (!response.ok) {
            throw new Error('Unable to update title of question');
        }
    } catch (e) {
        console.error(e);
        throw e;
    }
}

// Update range from question
export async function updateQuestionRange(questionId: string, min: string, max: string) {
    try {
        const response = await fetch(`/api/Questions/UpdateRangeQuestion?id=${questionId}&min=${min}&max=${max}`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            }
        });
        if (!response.ok) {
            throw new Error('Unable to update range of question');
        }
    } catch (e) {
        console.error(e);
        throw e;
    }
}

// Add option from to question
export async function postOption(questionId: string, newOption: string) {
    try {
        const response = await fetch(`/api/Options/AddOption/${questionId}`, {
            method: 'PUT',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(newOption)
        });
        if (!response.ok) {
            throw new Error('Unable to add option to question');
        }
    } catch (e) {
        console.error(e);
        throw e;
    }
}

// Add Media to question
export async function postMedia(formData: FormData) {
    try {
        const response = await fetch('/api/Questions/UploadMedia', {
            method: 'POST',
            body: formData
        });
        if (!response.ok) {
            throw new Error('Unable to post media');
        }
    } catch (e) {
        console.error(e);
        throw e;
    }
}

// Delete an option from question
export async function deleteOption(option: string) {
    try {
        const response = await fetch(`/api/Options/DeleteOption?id=${option}`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            }
        });
        if (!response.ok) {
            throw new Error('Unable to delete option from question');
        }
    } catch (e) {
        console.error(e);
        throw e;
    }
}

export async function reOrderQuestions(questionId: string, position: number) {
    try {
        const response = fetch(`/api/Questions/${questionId}/Reorder/${position}`, {
            method: 'POST',
        });
    } catch (e) {
        console.error(e);
        throw e;
    }
}

export async function createQuestion(text: string, type: string, flowId: string) {
    try {
        const response = await fetch('/api/Questions/Create', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({
                Text: text,
                Type: type,
                FlowId: flowId
            }),
        });
        const data = await response.json();
        const question: Question = {
            id: data.id,
            text: data.text,
            type: data.type
        };
        return question;
    } catch(reason)
    {
        alert("Error creating Question:" + reason);
        return null
    }
}

export async function getQuestionsByFlowIdAfterPosition(flowId: string, position: string) {
    const response = await fetch(`/api/Questions/RedirectableQuestions?flowId=${flowId}&position=${position}`);
    if (!response.ok) {
        throw Error(`Unable to get questions: ${response.status} ${response.statusText}`);
    }
    const responseData = await response.text();
    return JSON.parse(responseData);
}

export async function ChangeRedirectedIdFromOption(questionId: string, selectedOption: string, questionIdToRedirect: string) {
try {
    const response = await fetch(`/api/Options/ChangeRedirectedIdFromOption?questionId=${questionId}&selectedOption=${selectedOption}&nextQuestionId=${questionIdToRedirect}`, {
        method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            }
        });
        if (!response.ok) {
            throw new Error('Unable to add conditional question');
        }
    } catch (e) {
        console.error(e);
        throw e;
    }
}

export async function getOptionById(optionId: string) {
    const response = await fetch(`/api/Options/GetOptionById/${optionId}`);
    if (!response.ok) {
        throw Error(`Unable to get option: ${response.status} ${response.statusText}`);
    }
    return response.json();
}