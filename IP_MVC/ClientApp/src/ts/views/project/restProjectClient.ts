import {Project} from "../../models/Project.interface";

export async function getProject(id: string) {
    const response = await fetch(`/api/Projects/${id}`, {
        headers: {
            'Accept': 'application/json'
        }
    });
    if (!response.ok) {
        throw new Error(`Unable to get project with id ${id}`)
    }
    return response.json();
}

export async function updateProject(name: string, description: string, projectId: string, adminId: string) {
    const response = await fetch(`/api/Projects`, {
        method: 'PUT',
        headers: {
            Accept: 'application/json',
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({
            ProjectId: projectId,
            AdminId: adminId,
            NewName: name,
            NewDescription: description
        }),
    })
    
    if (!response.ok) {
        throw new Error(`Failed to update project: ${response.statusText}`);
    }
}

export async function createProject(name: string, description: string) {
    const response = await fetch(`/api/Projects/Create`, {
        method: 'POST',
        headers: {
            Accept: 'application/json',
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({
            Name: name,
            Description: description
        }),
    })
    if (!response.ok) {
        throw new Error(`Failed to create project: ${response.statusText}`);
    }
    const data = await response.json();
    const project: Project = {
        Id: data.id,
        NewName: data.name,
        NewDescription: data.description,
        AdminId: data.adminId
    };
    return project;
}