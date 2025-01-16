import {Chart, registerables, LinearScale, ChartTypeRegistry} from 'chart.js';
import {ChartData, ChartDataObject, ChartItem} from "../models/Analytics.interfaces";
import {OpenQuestion} from "../models/Questions.interfaces";

Chart.register(...registerables, LinearScale);

function setupAnalytiscsEventListeners(){
    const dropdownItems = document.querySelectorAll('.dropdown-item');
    dropdownItems.forEach(item => {
        item.addEventListener('click', handleFlowChange);
    });
}
async function handleFlowChange(e: Event) {
    const button = e.target as HTMLButtonElement;
    const flowId = button.dataset.flowId;

    if (flowId) {
        const AnalyticsResponse = await fetch(`/api/Analytics/GetFlowAnalytics/${flowId}`);
        const data = await AnalyticsResponse.json();
        renderCharts(data.chartData);
        renderOpenQuestions(data.openQuestions);

        const NotesResponse = await fetch(`/api/Analytics/GetNotesForFlow/${flowId}`);
        const notes = await NotesResponse.json();
        renderNotes(notes);
    }
}
function renderCharts(chartData: ChartData[]) {
    const chartsContainer = document.querySelector('#chartsContainer');
    if (chartsContainer instanceof HTMLElement) {
        chartsContainer.innerHTML = '';
        chartData.forEach((item: ChartData, index: number) => {
            const chartItem: ChartItem = {
                title: item.title,
                type: item.type as keyof ChartTypeRegistry,
                data: item.data as ChartDataObject
            };
            const chartElement = createChartElement(chartItem, index);
            chartsContainer.appendChild(chartElement);
        });
    }
}
function createChartElement(item: ChartItem, index: number): HTMLElement {
    const mainDiv = document.createElement('div');
    mainDiv.classList.add('col-sm-12', 'col-md-12', 'col-lg-12', 'col-xl-12', 'col-xxl-12', 'pb-3', 'px-3');

    const cardDiv = document.createElement('div');
    cardDiv.classList.add('card', 'border-1', 'border-black', 'little-card');

    const titelDiv = document.createElement('div');
    titelDiv.innerHTML = item.data.datasets[0].label;
    titelDiv.classList.add('text-center', 'py-2');

    cardDiv.appendChild(titelDiv);

    const cardBodyDiv = document.createElement('div');
    cardBodyDiv.classList.add('card-body', 'overflow-scroll', 'd-flex', 'justify-content-center', 'align-items-center');

    const rowDiv = document.createElement('div');
    rowDiv.classList.add('row', 'w-100', 'h-100');

    const canvasDiv = document.createElement('div');
    canvasDiv.classList.add('col-sm-12', 'col-md-6', 'col-lg-6', 'col-xl-6', 'col-xxl-6', 'chart-container', 'flex-grow-1', 'd-flex', 'justify-content-center', 'align-items-center');
    const canvas = document.createElement('canvas');
    canvas.id = `flowAnalytics${index}`;
    canvas.classList.add('mw-100', 'mh-100');
    canvasDiv.appendChild(canvas);

    const legendDiv = document.createElement('div');
    legendDiv.id = `legend${index}`;
    legendDiv.classList.add('col-sm-12', 'col-md-6', 'col-lg-6', 'col-xl-6', 'col-xxl-6', 'chart-legend', 'd-flex', 'flex-column', 'justify-content-center', 'align-items-center');

    rowDiv.appendChild(canvasDiv);
    rowDiv.appendChild(legendDiv);

    cardBodyDiv.appendChild(rowDiv);
    cardDiv.appendChild(cardBodyDiv);
    mainDiv.appendChild(cardDiv);

    const ctx = canvas.getContext('2d');

    if (ctx) {
        const isPieChart = item.type === 'pie' || item.type === 'doughnut';

        const chart = new Chart(ctx, {
            type: item.type,
            data: item.data,
            options: {
                plugins: {
                    legend: {
                        display: false
                    }
                },
                scales: !isPieChart ? {
                    x: {
                        ticks: {
                            display: false
                        }
                    },
                    y: {
                        beginAtZero: true
                    }
                } : {}
            }
        });

        const legendHTML = item.data.labels.map((label: string, i: number) => {
            const bgColor = item.data.datasets[0].backgroundColor[i];
            return `<div class="legend-item"><span class="legend-color d-inline-block" style="background-color:${bgColor};"></span>${label}</div>`;
        }).join('');
        legendDiv.innerHTML = legendHTML;
    }

    return mainDiv;
}
function renderOpenQuestions(openQuestions: OpenQuestion[]) {
    const chartsContainer = document.querySelector('#chartsContainer');
    if (chartsContainer instanceof HTMLElement) {
        openQuestions.forEach((item: OpenQuestion, index: number) => {
            if (item.question && item.answers && item.answers.length > 0) {
                const div = document.createElement('div');
                div.innerHTML = `<h2>${item.question}</h2><ul>${item.answers.filter((answer: string | null): answer is string => answer !== null).map((answer: string) => `<li>${answer}</li>`).join('')}</ul>`;
                chartsContainer.appendChild(div);
            }
        });
    }
}

function renderNotes(notes: any[]) {
    const notesContainer = document.querySelector('#notesContainer');
    if (notesContainer instanceof HTMLElement) {
        notesContainer.innerHTML = '';
        if (notes && notes.length > 0) {
            const titleElement = document.createElement('h2');
            titleElement.textContent = 'Notes';
            titleElement.style.textAlign = 'center';
            notesContainer.appendChild(titleElement);

            notes.forEach((note: any) => {
                const noteElement = document.createElement('p');
                noteElement.textContent = note.content;
                noteElement.style.textAlign = 'center';
                notesContainer.appendChild(noteElement);
            });
        }
    }
}

setupAnalytiscsEventListeners();