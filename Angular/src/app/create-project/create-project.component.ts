import { Component, OnInit } from '@angular/core';
import { MatDialogRef } from '@angular/material';
import { ProjectService } from '../project/shared/project.service';

@Component({
  selector: 'app-create-project',
  templateUrl: './create-project.component.html',
  styleUrls: ['./create-project.component.less'],
  providers: [ProjectService]
})
export class CreateProjectComponent implements OnInit {

  constructor(public dialogRef: MatDialogRef<CreateProjectComponent>,
     public service: ProjectService) { }

  ngOnInit() {
    
  }

  onClose() {
    this.service.form.reset();
    this.service.initializeFormGroup();
    this.dialogRef.close();
  }

  onSubmit() {
    if (this.service.form.valid) {
      if (!this.service.form.get('id').value)
        this.service.addProject(this.service.form.value);
      else
      this.service.editProject(this.service.form.value);
      this.onClose();
    }
    else {
      const controls = this.service.form.controls;
      Object.keys(controls)
       .forEach(controlName => controls[controlName].markAsTouched());
       return;
     }
  }
}