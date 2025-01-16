export function fetchFacilitators(searchTerm: string) {
    return fetch(`/api/Projects/ManageFacilitators?searchTerm=${searchTerm}`)
        .then(response => response.json());
}
export function addUserToProject(user: string, projectId: string): Promise<Response> {
    return fetch(`/api/Projects/AddUser`, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({ userName: user, projectId: projectId })
    });
}