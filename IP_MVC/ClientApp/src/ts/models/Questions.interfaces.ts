export interface Question {
    id: number;
    text: string;
    type: string;
}
export interface OpenQuestion {
    question: string;
    answers: (string | null)[];
}