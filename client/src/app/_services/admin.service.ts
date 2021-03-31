import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { AnyARecord } from 'dns';
import { environment } from 'src/environments/environment';
import { Photo } from '../_models/photo';
import { User } from '../_models/user';

@Injectable({
  providedIn: 'root'
})
export class AdminService {
  baseUrl = environment.apiUrl;

  constructor(private http: HttpClient) { }

  getUsersWithRoles(): any {
    return this.http.get<Partial<User[]>>(this.baseUrl + 'admin/users-with-roles');
  }

  updateUserRoles(username: string, roles: string[]): any {
    return this.http.post(this.baseUrl + 'admin/edit-roles/' + username + '?roles=' + roles, {});
  }

  getPhotoForApproval(){
    return this.http.get<Partial<Photo[]>>(this.baseUrl + 'admin/photos-to-moderate', {});
  }

  approvePhoto(photoid: number): any{
    return this.http.put(this.baseUrl + 'admin/aprove-photo/' + photoid, {});
  }

  rejectPhoto(photoid: number): any{
    return this.http.put(this.baseUrl + 'admin/reject-photo/' + photoid, {});
  }
}
