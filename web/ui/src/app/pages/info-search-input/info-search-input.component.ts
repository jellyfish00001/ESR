import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-info-search-input',
  templateUrl: './info-search-input.component.html',
  styleUrls: ['./info-search-input.component.scss']
})
export class InfoSearchInputComponent implements OnInit {

  constructor() { }

  ngOnInit(): void {
  }

  getSearchList(value){
    console.log(value)
  }
}
