import { AngularMultiSelectModule } from 'angular2-multiselect-dropdown';
import { GetTitleService } from './../../shared/service/get-title.service';
import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { IdmService } from 'src/app/shared/service/idm.service';
import { Subscription } from 'rxjs';
AngularMultiSelectModule;
@Component({
  selector: 'app-token-info',
  templateUrl: './token-info.component.html',
  styleUrls: ['./token-info.component.scss'],
})
export class TokenInfoComponent implements OnInit {
  public postMessage_value: any;
  postMessage_keys;
  subscription: Subscription;
  dropdownList = [];
  selectedItems = [];
  dropdownSettings = {};
  constructor(
    public activatedRoute: ActivatedRoute,
    public titleService: GetTitleService,
      private idmService: IdmService
  ) {}
  ngOnInit(): void {
    const snpData = this.activatedRoute.snapshot.data;
    this.titleService.setTitle(snpData?.title);

    this.subscription = this.idmService.receiveMsgSender.subscribe(
      (data: any) => {
        this.postMessage_value = data;
        this.postMessage_keys = Object.keys(data);
      }
    );
    this.dropdownList = [
      { id: 1, itemName: 'India' },
      { id: 2, itemName: 'Singapore' },
      { id: 3, itemName: 'Australia' },
      { id: 4, itemName: 'Canada' },
      { id: 5, itemName: 'South Korea' },
      { id: 6, itemName: 'Germany' },
      { id: 7, itemName: 'France' },
      { id: 8, itemName: 'Russia' },
      { id: 9, itemName: 'Italy' },
      { id: 10, itemName: 'Sweden' },
    ];
    this.selectedItems = [
      { id: 1, itemName: 'India' },
      { id: 2, itemName: 'Singapore' },
      { id: 3, itemName: 'Australia' },
      { id: 4, itemName: 'Canada' },
      { id: 5, itemName: 'South Korea' },
      { id: 6, itemName: 'Germany' },
      { id: 7, itemName: 'France' },
      { id: 8, itemName: 'Russia' },
      { id: 9, itemName: 'Italy' },
      { id: 10, itemName: 'Sweden' },
    ];

    this.dropdownSettings = {
      singleSelection: false,
      text: '請選擇',
      selectAllText: 'Select All',
      unSelectAllText: 'UnSelect All',
      enableSearchFilter: false,
      classes: 'myclass custom-class',
      badgeShowLimit: 2, // 限定顯示選項的數量
      position: 'top',
    };
  }
  onItemSelect(item: any) {
    console.log(item);
    console.log(this.selectedItems);
  }
  OnItemDeSelect(item: any) {
    console.log(item);
    console.log(this.selectedItems);
  }
  onSelectAll(items: any) {
    console.log(items);
  }
  onDeSelectAll(items: any) {
    console.log(items);
  }
}
