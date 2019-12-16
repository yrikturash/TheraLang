import { __decorate } from "tslib";
import { Injectable } from "@angular/core";
import { projectUrl } from "../shared/api-endpoint.constants";
let ProjectRequestService = class ProjectRequestService {
    constructor(http) {
        this.http = http;
        this.url = projectUrl;
    }
    getAllProjectParticipants() {
        return this.http.get(this.url);
    }
    GetStatusChangetOfProjects(status) {
        return this.http.get(this.url + "/newstatus/" + status);
    }
    Approved(id) {
        return this.http.get(this.url + "/approve/" + id);
    }
    Rejected(id) {
        return this.http.get(this.url + "/reject/" + id);
    }
};
ProjectRequestService = __decorate([
    Injectable()
], ProjectRequestService);
export { ProjectRequestService };
//# sourceMappingURL=project-request.service.js.map