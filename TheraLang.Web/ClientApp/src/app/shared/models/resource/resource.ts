import { ResourceProjects } from "./resource-projects";
import { ResourceCategory } from "./resource-category";
import { User } from "../user/User";

export class Resource {
  toLowerCase() {
    throw new Error("Method not implemented.");
  }
  user: User;
  name: string;
  description: string;
  url?: any;
  file?: any;
  categoryId: number;
  resourceCategory: ResourceCategory;
  resourceProjects: ResourceProjects;
  id: number;
  createdById: number;
  updatedById: number;
  createdDateUtc: Date;
  updatedDateUtc?: any;
}
