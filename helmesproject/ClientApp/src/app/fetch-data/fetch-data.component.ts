import { Component, Inject, OnInit, ViewChild  } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { MatTable, MatTableDataSource } from '@angular/material/table';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { animate, state, style, transition, trigger } from '@angular/animations';

@Component({
  selector: 'app-fetch-data',
  templateUrl: './fetch-data.component.html',
  animations: [
    trigger('detailExpand', [
      state('collapsed', style({ height: '0px', minHeight: '0' })),
      state('expanded', style({ height: '*' })),
      transition('expanded <=> collapsed', animate('225ms cubic-bezier(0.4, 0.0, 0.2, 1)')),
    ]),
  ],
})
export class FetchDataComponent implements OnInit{
  news: NewsItem[] = [];
  httpClient: HttpClient;
  baseUrl: string;
  interval: any;
  columnsToDisplay: string[] = ['date', 'title', 'author', 'url'];
  dataSource = new MatTableDataSource<NewsItem>(this.news);
  isLoadingResults = false;
  expandedElement: NewsItem | null | undefined;
  columnsToDisplayWithExpand = [...this.columnsToDisplay, 'expand'];



  @ViewChild(MatTable) newsTable!: MatTable<NewsItem>;
  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort;
 

  constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    this.httpClient = http;
    this.baseUrl = baseUrl;
  }

  ngOnInit() {
    this.refreshData();
   
    this.interval = setInterval(() => {
      this.refreshData();
    }, 30000);
  }

  ngAfterViewInit() {
    this.dataSource.paginator = this.paginator;
    this.dataSource.sort = this.sort;
  }

  refreshData() {

    //Lai lieki vizuāli neraustās lapa, jo no tā loadera tāpat nav jēga pie maza ierakstu skaita
    if (this.dataSource.data.length > 1000) {
      this.isLoadingResults = true;
    }
      
    this.httpClient.get<NewsItem[]>(this.baseUrl + 'news').subscribe(result => {
      this.news = result;
      this.dataSource.data = this.news;
      this.isLoadingResults = false;

    }, error => console.error(error));
  }

  applyFilter(event: Event) {
    const filterValue = (event.target as HTMLInputElement).value;
    this.dataSource.filter = filterValue.trim().toLowerCase();
  }

  convertDate(date: string) {
    var newDate = new Date(date);
    return newDate.toLocaleString([], { year: 'numeric', month: 'numeric', day: 'numeric', hour: '2-digit', minute: '2-digit' });
  }
}


interface NewsItem {
  id: number;
  author: string;
  title: string;
  date: string;
  itemguid: string;
  readmoreurl: string;
  content: string;
  url: string;
  categoryid: number;
}
