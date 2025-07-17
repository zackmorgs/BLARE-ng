import { Injectable } from '@angular/core';
import { Title } from '@angular/platform-browser';
import { Router, NavigationEnd, ActivatedRoute } from '@angular/router';
import { filter, map } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class TitleService {

  constructor(
    private title: Title,
    private router: Router,
    private activatedRoute: ActivatedRoute
  ) {
    this.initTitleListener();
  }

  private initTitleListener() {
    this.router.events
      .pipe(
        filter(event => event instanceof NavigationEnd),
        map(() => {
          let route = this.activatedRoute;
          while (route.firstChild) {
            route = route.firstChild;
          }
          return route;
        }),
        map(route => route.snapshot.data['title'] || 'BLARE')
      )
      .subscribe(title => {
        this.title.setTitle(title);
      });
  }

  setTitle(title: string) {
    this.title.setTitle(title);
  }

  setDynamicTitle(baseTitle: string, dynamicPart?: string) {
    const fullTitle = dynamicPart 
      ? `${dynamicPart} - ${baseTitle} - BLARE`
      : `${baseTitle} - BLARE`;
    this.title.setTitle(fullTitle);
  }
}
