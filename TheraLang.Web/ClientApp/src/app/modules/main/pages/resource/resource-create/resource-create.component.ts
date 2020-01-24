import { Component, OnInit } from "@angular/core";
import { MatDialogRef } from "@angular/material/dialog";
import { ResourceCategory } from "src/app/shared/models/resource/resource-category";
import { ResourceCreateService } from "src/app/core/http/resource/resource-create.service";
import { Resource } from "src/app/shared/models/resource/resource";

@Component({
  selector: "app-resource-create",
  templateUrl: "./resource-create.component.html",
  styleUrls: ["./resource-create.component.less"]
})
export class ResourceCreateComponent implements OnInit {
  categories: ResourceCategory[];
  fileName : string;

  constructor(
    private dialog: MatDialogRef<ResourceCreateComponent>,
    public service: ResourceCreateService
  ) {}

  ngOnInit() {
    this.service
      .getCategories()
      .subscribe(
        (categories: ResourceCategory[]) => (this.categories = categories)
      );
  }

  onClose() {
    this.service.resourceForm.reset();
    this.service.initializeForm();
    this.dialog.close();
  }

  onSubmit() {
    if (this.service.resourceForm.invalid) {
      const controls = this.service.resourceForm.controls;
      Object.keys(controls).forEach(controlName =>
        controls[controlName].markAsTouched()
      );
      return;
    } else {
      const resource: Resource = this.service.resourceForm.value;
      resource.file = this.service.resourceForm.value.file.files[0];
      this.service.addResource(resource);
      this.onClose();
    }
  }
}
