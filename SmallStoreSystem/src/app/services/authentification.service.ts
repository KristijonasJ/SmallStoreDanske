import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { map, Observable, tap } from 'rxjs';


@Injectable({
  providedIn: 'root'
})
export class AuthentificationService {


  constructor() { }

  private loggedIn = false;
  AdminUser = false;
  User = false;
  userType: string = '';
  isValidUser: boolean = false;

  seeUserInfo() {
    if(environment.Admin_Access == 'Valid') {
      this.AdminUser = true;
      this.User = false;
      this.userType = 'Admin';
      this.isValidUser = true;
    }
    else if (environment.User_Access == 'Valid') {
      this.User = true;
      this.AdminUser = false;
      this.userType = 'User';
      this.isValidUser = true;
    }
  }
  
}
