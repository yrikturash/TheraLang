import { Component, OnInit, Input } from '@angular/core';
import { NewsDetails } from 'src/app/shared/models/news/newsDetails';
import { ActivatedRoute, Router } from '@angular/router';
import { NewsService } from 'src/app/core/http/news/news.service';
import { DialogService } from 'src/app/core/services/dialog/dialog.service';
import { TranslateService } from '@ngx-translate/core';
import { NewsPreview } from 'src/app/shared/models/news/newsPreview';
import { NotificationService } from 'src/app/core/services/notification/notification.service';
import { UserService } from 'src/app/core/auth/user.service';
import { AuthService } from "../../../../../core/auth/auth.service";

@Component({
  selector: 'app-news-details',
  templateUrl: './news-details.component.html',
  styleUrls: ['./news-details.component.less']
})
export class NewsDetailsComponent implements OnInit {
  newsId: number;
  newsDetails: NewsDetails;
  contentImages: Array<object> = [];
  values: any;
  news: NewsPreview[];


  constructor(
    private route: ActivatedRoute,
    private service: NewsService,
    private dialogService: DialogService,
    private translate: TranslateService,
    private notificationService: NotificationService,
    public userService: UserService,
    public authService: AuthService,
    private router: Router) {
  }

  ngOnInit() {
    this.newsId = parseInt(this.route.snapshot.paramMap.get("newsId"));
    this.getNewsById(this.newsId);
  }

  getNewsById(id: number) {
    this.service.getNewsById(id).subscribe((data: NewsDetails) => {
      this.newsDetails = data;
      this.newsDetails.contentImageUrls.map(image => {
        this.contentImages.push({
          image: image,
          thumbImage: image
        });
      });
    });
  }

  async onDelete(newsId: number) {
    this.dialogService
      .openConfirmDialog(
        await this.translate.get("common.r-u-sure").toPromise()
      )
      .afterClosed()
      .subscribe(async res => {
        if (res) {
          this.service.deleteNews(newsId).subscribe(result => {
            this.service
              .getAllNews()
              .subscribe(async (news: NewsPreview[]) => {
                this.news = news;
                this.notificationService.success(await this.translate.get("common.deleted-successfully").toPromise());
                await this.router.navigateByUrl("/news");
              });
          });

        }
      });

  }

  isAuthenticated() {
    return this.userService.isAuthenticated();
  }

  async like(){
    if(!this.isAuthenticated()){
      this.notificationService.info(await this.translate.get("common.messages.register-first").toPromise());
      return
    }
    if(this.newsDetails.isLikedByCurrentUser){
      this.newsDetails.likesCount--;
    }
    else{
      this.newsDetails.likesCount++;
    }
    this.service.like(this.newsId).subscribe();
    this.newsDetails.isLikedByCurrentUser=!this.newsDetails.isLikedByCurrentUser;
  }

}
