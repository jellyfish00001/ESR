import { Component, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Subscription } from 'rxjs';
import { MatomoSetUserid } from 'src/app/shared/decorators/matomo-set-userid.decorator';
import { TrackPageView } from 'src/app/shared/decorators/matomo-track-page-view.decorator';
import { GetTitleService } from 'src/app/shared/service/get-title.service';
import { IdmService } from 'src/app/shared/service/idm.service';

@Component({
  selector: 'app-sendmail',
  templateUrl: './sendmail.component.html',
  styleUrls: ['./sendmail.component.scss'],
})
export class SendmailComponent implements OnInit, OnDestroy {
  public postMessage_value: any;
  postMessage_keys;
  subscription: Subscription;
  constructor(
    public activatedRoute: ActivatedRoute,
    public titleService: GetTitleService,
    private idmService: IdmService
  ) {}

  @TrackPageView('send mail 發送郵件')
  @MatomoSetUserid(localStorage.getItem('userId'))
  public ngOnInit(): void {
    // debugger
    const snpData = this.activatedRoute.snapshot.data;
    this.titleService.setTitle(snpData?.title);
    this.subscription = this.idmService.receiveMsgSender.subscribe(
      (data: any) => {
        this.postMessage_value = data;
        this.postMessage_keys = Object.keys(data);
      }
    );
  }

  ngOnDestroy(): void {
    //Called once, before the instance is destroyed.
    //Add 'implements OnDestroy' to the class.
    if (this.subscription) {
      this.subscription.unsubscribe();
    }
  }
}
