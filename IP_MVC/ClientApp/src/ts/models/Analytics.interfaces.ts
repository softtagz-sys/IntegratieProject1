import {ChartTypeRegistry} from "chart.js";

export interface ChartData {
    title: string;
    type: string;
    data: number[] | object;
}
export interface Dataset {
    backgroundColor: string[];
    borderColor: string[];
    borderWidth: number;
    data: number[];
    label: string;
}
export interface ChartDataObject {
    datasets: Dataset[];
    labels: string[];
}
export interface ChartItem {
    title: string;
    type: keyof ChartTypeRegistry;
    data: ChartDataObject;
}
