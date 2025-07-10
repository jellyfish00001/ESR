import { Component, OnInit, ViewChild } from '@angular/core';
import {
  UntypedFormBuilder,
  FormControl,
  UntypedFormGroup,
  Validators,
} from '@angular/forms';
import { TranslateService } from '@ngx-translate/core';
import { NzModalService } from 'ng-zorro-antd/modal';
import { URLConst } from 'src/app/shared/const/url.const';
// import { AuthService } from 'src/app/shared/service/auth.service';
import { WebApiService } from 'src/app/shared/service/webapi.service';
import { EnvironmentconfigService } from '../../shared/service/environmentconfig.service';
import { NzMessageService } from 'ng-zorro-antd/message';
import {
  NzShowUploadList,
  NzUploadChangeParam,
  NzUploadFile,
  UploadFilter,
} from 'ng-zorro-antd/upload';
import { format } from 'date-fns';
import { Guid } from 'guid-typescript';
import { Observable, Observer } from 'rxjs';
import { ActivatedRoute, Router } from '@angular/router';
import { CryptoService } from 'src/app/shared/service/crypto.service';
import { CommonService } from 'src/app/shared/service/common.service';
import { ApprovalParams } from 'src/app/ERS-Pages/rq204/classes/data-item';
import {
  ExpenseTableColumn,
  OvertimeMealExpenseTableColumn,
  DriveFuelExpenseTableColumn,
} from '../rq501/classes/table-column';
import { GeneralExpenseInfo } from '../rq501/classes/data-item';

@Component({
  selector: 'app-rq504',
  templateUrl: './rq504.component.html',
  styleUrls: ['./rq504.component.scss'],
})
export class RQ504Component implements OnInit {
  nzFilterOption = () => true;
  rno: string;
  screenHeight: any;
  screenWidth: any;
  infoForm: UntypedFormGroup;
  companyList: any[] = [];
  detailListTableColumn = ExpenseTableColumn;
  detailTableData: GeneralExpenseInfo[] = [];
  isSpinning = false;
  queryParam: any;
  userInfo: any;
  invoiceFileList: any[] = [];
  attachmentList: any[] = [];
  attachList: any[] = [];
  isSaveLoading = false;
  employeeList: any[] = [];
  exTotalWarning: any[] = [];
  isFinUser: boolean = false;
  type: string = null;
  preUrl: string;
  showTransformButton: boolean = false;
  currency: string = 'RMB';

  constructor(
    private fb: UntypedFormBuilder,
    private Service: WebApiService,
    // private authService: AuthService,
    private modal: NzModalService,
    public translate: TranslateService,
    private EnvironmentconfigService: EnvironmentconfigService,
    private message: NzMessageService,
    private router: Router,
    private actRoute: ActivatedRoute,
    private crypto: CryptoService,
    private commonSrv: CommonService
  ) {}

  ngOnInit(): void {
    this.isSpinning = true;
    this.infoForm = this.fb.group({
      companyCode: [{ value: null, disabled: true }],
      dept: [{ value: null, disabled: true }],
      expenseProject: [{ value: null, disabled: true }],
      rno: [{ value: null, disabled: true }],
      ext: [{ value: null, disabled: true }],
      projectCode: [{ value: null, disabled: true }],
      applicantEmplid: [{ value: null, disabled: true }],
      applicantName: [{ value: null, disabled: true }],
      scene: [{ value: null, disabled: true }],
      fileCategory: [{ value: null, disabled: true }],
      totalAmt: [{ value: 0, disabled: true }],
      actualAmt: [{ value: 0, disabled: true }],
    });
    this.getEmployeeInfo();
    this.getRnoInfo();
  }

  autoTips: Record<string, Record<string, string>> = {
    default: {
      required: this.translate.instant('can-not-be-null'),
    },
  };

  getEmployeeInfo() {
    this.userInfo = this.commonSrv.getUserInfo;
    if (!this.userInfo) {
      this.message.error(
        'Can not get user information. Please refresh the page...',
        { nzDuration: 6000 }
      );
    }
  }

  getRnoRole() {
    this.Service.Post(
      this.EnvironmentconfigService.authConfig.ersUrl +
        URLConst.CheckIsAccount ,
      null
    ).subscribe((res) => {
      if (!!res && res.status === 200 && !!res.body) {
        if (res.body.status != 1) {
          this.message.error(res.body.message, { nzDuration: 6000 });
        } else {
          this.isFinUser = res.body.data;
          this.showTransformButton = res.body.data == 1;
          if (this.isFinUser) {
            this.infoForm.controls.actualAmt.enable();
          }
        }
      } else if (res?.status !== 200) {
        this.message.error(res.message, { nzDuration: 6000 });
      }
    });
  }

  getRnoInfo() {
    this.actRoute.queryParams.subscribe((res) => {
      if (res && res.data && JSON.parse(res.data)) {
        let data = JSON.parse(res.data);
        let rno: string = this.crypto.decrypt(data.rno);
        this.infoForm.controls.rno.setValue(rno);
        this.rno = rno;
        this.getRnoRole();
        this.preUrl = data.preUrl;
        // 获取单据信息
        // this.Service.Post('http://localhost:5000' + URLConst.GetFormData, { rno: rno }).subscribe((res) => {
        this.Service.Post(
          this.EnvironmentconfigService.authConfig.ersUrl +
            URLConst.GetFormData,
          { rno: rno }
        ).subscribe((res) => {
          if (res != null && res.status === 200 && !!res.body) {
            if (res.body.status == 1) {
              this.assembleFromData(res.body.data);
              this.getSceneList();
            } else {
              this.message.error(res.body.message, { nzDuration: 6000 });
            }
          } else {
            this.message.error(
              res.message ?? this.translate.instant('server-error'),
              { nzDuration: 6000 }
            );
          }
        });
      }
    });
  }

  assembleFromData(value): void {
    let headData = value.head;
    let listData = value.detail;
    let allFileList = value.file;
    let invoiceDetailList = value.invList;
    let summaryAtmData = value.amount;
    if (headData != null) {
      this.infoForm.controls.companyCode.setValue(headData.company);
      this.infoForm.controls.expenseProject.setValue(headData.dtype);
      this.infoForm.controls.projectCode.setValue(headData.projectcode);
      this.infoForm.controls.applicantEmplid.setValue(headData.payeeId);
      this.infoForm.controls.applicantName.setValue(headData.payeename);
      this.infoForm.controls.dept.setValue(headData.deptid);
      this.infoForm.controls.ext.setValue(headData.ext);
    }
    if (summaryAtmData != null) {
      this.infoForm.controls.totalAmt.setValue(summaryAtmData.amount);
      this.infoForm.controls.actualAmt.setValue(summaryAtmData.actamt);
      this.currency = summaryAtmData.currency;
    }

    if (listData != null) {
      this.infoForm.controls.scene.setValue(listData[0].expcode);
      this.attachmentList = allFileList
        .filter((f) => f.ishead == 'Y' && f.status != 'F' && f.status != 'I')
        .sort((a, b) => b.item - a.item)
        .map((f) => {
          return {
            id: f.seq,
            uid: f.item.toString(),
            name: f.filename,
            filename: f.tofn,
            status: 'done',
            url: this.commonSrv.changeDomain(f.url),
            type: f.filetype,
            originFileObj: null,
          };
        });
      this.invoiceFileList = allFileList
        .filter((f) => f.ishead == 'Y' && f.status == 'I')
        .sort((a, b) => b.item - a.item)
        .map((f) => {
          return {
            id: f.seq,
            uid: f.item.toString(),
            name: f.filename,
            filename: f.tofn,
            status: 'done',
            url: this.commonSrv.changeDomain(f.url),
            type: f.filetype,
            originFileObj: null,
            category: f.category,
            invoiceno: f.invoiceno,
          };
        });
      this.attachList = allFileList
        .filter((f) => f.ishead == 'Y' && f.status == 'F')
        .sort((a, b) => b.item - a.item)
        .map((f) => {
          return {
            id: f.seq,
            uid: Guid.create().toString(),
            name: f.filename,
            filename: f.tofn,
            status: 'done',
            url: this.commonSrv.changeDomain(f.url),
            type: f.filetype,
            category: f.category,
            source: '',
            originFileObj: null,
          };
        });
      if (this.attachList.length > 0) {
        let fileCategory = this.attachList[0].category;
        if (!fileCategory && !listData[0].expcode) {
          this.Service.doGet(
            this.EnvironmentconfigService.authConfig.ersUrl +
              URLConst.GetFileRequest,
            {
              expcode: listData[0].expcode,
              company: this.infoForm.controls.companyCode.value,
              category: 1,
            }
          ).subscribe((res) => {
            if (res && res.status === 200 && !!res.body) {
              if (res.body.status == 1 && res.body.data?.length > 0) {
                let result = res.body.data[0];
                fileCategory = result.filecategory;
              } else {
                this.message.error(res.body.message, { nzDuration: 6000 });
              }
            } else {
              this.message.error(
                res.message ?? this.translate.instant('server-error'),
                { nzDuration: 6000 }
              );
            }
          });
        }
        this.infoForm.controls.fileCategory.setValue(fileCategory);
      }
      listData.map((o) => {
        this.detailTableData.push({
          attribDept: o.deptid,
          startingPlace: o.location,
          cityOnBusiness: o.city,
          feeDate: format(new Date(o.rdate), 'yyyy/MM/dd'),
          carType: o.cartype,
          carTypeName: null,
          kil: o.journey,
          digest: o.summary,
          startingTime: format(new Date(o.gotime), 'HH:mm'),
          backTime: format(new Date(o.backtime), 'HH:mm'),
          curr: o.currency,
          expenseAmt: o.amount1,
          exchangeRate: o.rate,
          toLocalAmt: o.baseamt,
          id: o.seq,
          disabled: false,
          payeeId: o.payeeid,
          payeeName: o.payeename,
          payeeDeptid: o.payeedeptid,
          bankName: o.bank,
          whiteRemark: o.assignment,
          taxexpense: o.taxexpense,
        });
      });
      let index = 1;
      invoiceDetailList.map((o) => {
        if (o.expcode != null && o.expcode != '') {
          this.exTotalWarning.push(index++ + '. ' + o.expcode);
        }
      });
      this.detailTableData = this.detailTableData.sort((a, b) => a.id - b.id);
      this.detailTableData = [...this.detailTableData];
    }
    this.isSpinning = false;
  }

  getSceneList() {
    this.Service.doGet(
      this.EnvironmentconfigService.authConfig.ersUrl + URLConst.GetSceneList,
      { company: this.infoForm.controls.companyCode.value }
    ).subscribe((res) => {
      if (res && res.status === 200 && !!res.body) {
        if (res.body.status == 1) {
          var sceneList = res.body.data;
          if (this.detailTableData.length > 0) {
            let sceneData = sceneList.filter(
              (o) => o.expcode == this.infoForm.controls.scene.value
            )[0];
            this.infoForm.controls.scene.setValue(sceneData.expname);
            if (sceneData.expcategory == 0) {
              this.type = 'default';
              this.detailListTableColumn = ExpenseTableColumn;
            } else if (sceneData.expcategory == 1) {
              this.type = 'overtimeMeal';
              this.detailListTableColumn = OvertimeMealExpenseTableColumn;
            } else {
              this.type = 'drive';
              this.getCarType();
              this.detailListTableColumn = DriveFuelExpenseTableColumn;
            }
          }
        } else {
          this.message.error(res.body.message, { nzDuration: 6000 });
        }
      } else {
        this.message.error(
          res.message ?? this.translate.instant('server-error'),
          { nzDuration: 6000 }
        );
      }
    });
  }

  getCarType() {
    this.Service.doGet(
      this.EnvironmentconfigService.authConfig.ersUrl + URLConst.GetCarTypeList,
      { company: this.infoForm.controls.companyCode.value }
    ).subscribe((res) => {
      if (res && res.status === 200 && !!res.body) {
        if (res.body.status == 1) {
          var carTypeList = res.body.data;
          if (this.detailTableData.length > 0) {
            this.detailTableData
              .filter((o) => o.carTypeName == null)
              .map((o) => {
                o.carTypeName = carTypeList.filter(
                  (f) => f.type == o.carType
                )[0]?.name;
              });
          }
        } else {
          this.message.error(res.body.message, { nzDuration: 6000 });
        }
      } else {
        this.message.error(
          res.message ?? this.translate.instant('server-error'),
          { nzDuration: 6000 }
        );
      }
    });
  }

  showNoElectronicFileTips(): void {
    this.message.info(this.translate.instant('tips-no-electronic-file'), {
      nzDuration: 5000,
    });
  }

  approve(data: ApprovalParams): void {
    this.isSpinning = true;
    if (this.isFinUser) {
      data['detail'] = this.detailTableData.map((o) => {
        return {
          seq: o.id,
          assignment: o.whiteRemark,
          taxexpense: o.taxexpense,
        };
      });
    }
    this.commonSrv.Approval(data, this.preUrl);
    this.isSpinning = false;
  }
  reject(data: ApprovalParams): void {
    this.isSpinning = true;
    if (this.isFinUser) {
      data['detail'] = this.detailTableData.map((o) => {
        return {
          seq: o.id,
          assignment: o.whiteRemark,
          taxexpense: o.taxexpense,
        };
      });
    }
    this.commonSrv.Reject(data, this.preUrl);
    this.isSpinning = false;
  }
  transform(data: ApprovalParams): void {
    this.isSpinning = true;
    if (this.isFinUser) {
      data['detail'] = this.detailTableData.map((o) => {
        return {
          seq: o.id,
          assignment: o.whiteRemark,
          taxexpense: o.taxexpense,
        };
      });
    }
    this.commonSrv.Transform(data, this.preUrl);
    this.isSpinning = false;
  }
}
