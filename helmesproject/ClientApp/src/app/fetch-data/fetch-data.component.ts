import { Component, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-fetch-data',
  templateUrl: './fetch-data.component.html'
})
export class FetchDataComponent {
  news: NewsItem[] = [];
  page = 1;
  pageSize = 10;
  pageSizes = [5, 10, 20, 50, 100];

  constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    http.get<NewsItem[]>(baseUrl + 'news').subscribe(result => {
      this.news = result;
    }, error => console.error(error));
  
  }

  convertDate(date: string) {
    var newDate = new Date(date);
    return newDate.toLocaleString();
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
