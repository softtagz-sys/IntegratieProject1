import * as client from "../question/restQuestionClient";
import Sortable from "sortablejs";

async function reorderQuestions() {
    const sortable = document.getElementById('sortable');
    if (sortable) {
            new Sortable(sortable, {
                onUpdate: function (evt) {
                    const questionElement = evt.item.querySelector('.question');
                    if (questionElement) {
                        const questionId = questionElement.getAttribute('data-question-id') as string;
                        const position = 1 + <number>evt.newIndex;
                        client.reOrderQuestions(questionId, position);
                    }
                },
            });
    }
}

reorderQuestions();