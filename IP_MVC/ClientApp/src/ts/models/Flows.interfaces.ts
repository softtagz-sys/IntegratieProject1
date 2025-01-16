export interface Flow {
    NewName: string;
    NewDescription: string;
    NewStartDate: Date;
    NewEndDate: Date;
    NewProjectId: number;
    NewParentFlowId: number | null;
    IsParentFlow: boolean; 
}