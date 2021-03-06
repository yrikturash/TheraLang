import { Component, OnInit, AfterViewInit, OnDestroy } from "@angular/core";
import { ProjectParticipationRequest } from "src/app/shared/models/project-participation/project-participation-request";
import { Subscription } from "rxjs";
import { ProjectParticipationService } from "../../../core/http/project-participants/project-participation.service";
import { EventService } from "../../../core/services/event/event-service";
import { DialogService } from "../../../core/services/dialog/dialog.service";
import { AuthService } from "../../../core/auth/auth.service";
import { ProjectParticipationRequestStatus } from "src/app/configs/project-participation-request-status";
import { LoginComponent } from 'src/app/modules/login/login.component';

@Component({
  selector: "app-toolbar",
  templateUrl: "./toolbar.component.html",
  styleUrls: ["./toolbar.component.less", "./toolbar-menu-item.less"]
})
export class ToolbarComponent implements OnInit, AfterViewInit, OnDestroy {
  hasNotification: boolean = false;
  projectParticipation: ProjectParticipationRequest[];

  private subscription = new Subscription();

  constructor(
    private participantService: ProjectParticipationService,
    private eventService: EventService,
    private dialog: DialogService,
    private userService: AuthService
  ) {}

  ngOnInit() {
    const subscription = this.participantService
      .getAllProjectParticipants()
      .subscribe((projectParticipation: ProjectParticipationRequest[]) => {
        this.projectParticipation = projectParticipation;
        if (
          this.projectParticipation.filter(
            x => x.status === ProjectParticipationRequestStatus.New
          ).length > 0
        ) {
          this.hasNotification = true;
        }
      });
    this.subscription.add(subscription);
  }

  ngAfterViewInit(): void {
    this.eventService.childEventListner().subscribe(click => {
      this.hasNotification = false;
    });
  }

  ngOnDestroy(): void {
    this.subscription.unsubscribe();
  }

  onLogin() {
    this.dialog.openFormDialog(LoginComponent);
  }

  isAuthenticated(){
       return this.userService.isAuthenticated();
  }

  isAdmin(){
    return this.userService.isAdmin();
  }
}
