import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs";
import { NotificationsDetailsModel, NotificationRequestModel, Util, UserNotificationSettingModel, FollowedProduct } from '../models/';

@Injectable()
export class NotificationsService {
    constructor(private http: HttpClient, private util: Util) { }

    createNotifications(notificationRequestList: NotificationRequestModel[]) {
        return this.http.post(this.util.baseUrl + '/api/notifications/createNotifications/', notificationRequestList)
            .pipe(data => {
                return data;
            });
    }

    getUserNotifications(userId: number, page: number): Observable<NotificationsDetailsModel> {
        return this.http.get<NotificationsDetailsModel>(this.util.baseUrl + '/api/notifications/getUserNotifications/' + userId + '/' + page)
            .pipe(data => {
                return data;
            });
    }

    setUserNotificationAsSeen(notificationId: number) {
        return this.http.post(this.util.baseUrl + '/api/notifications/setUserNotificationAsSeen/', notificationId)
            .pipe(data => {
                return data;
            });
    }

    getUserNotificationsSettings(userId: number): Observable<UserNotificationSettingModel[]> {
        return this.http.get<UserNotificationSettingModel[]>(this.util.baseUrl + '/api/notifications/getUserNotificationsSettings/' + userId)
            .pipe(data => {
                return data;
            });
    }

    saveUserNotificationsSettings(userId: number, notificationsSettings: UserNotificationSettingModel[]) {
        return this.http.post(this.util.baseUrl + '/api/notifications/saveUserNotificationsSettings/', { userId, notificationsSettings })
            .pipe(data => {
                return data;
            });
    }

    getFollowedProducts(): Observable<FollowedProduct[]> {
        return this.http.get<FollowedProduct[]>(this.util.baseUrl + '/api/notifications/getFollowedProducts/')
            .pipe(data => {
                return data;
            });
    }

    getRetailersFollowingProduct(weedId: number, basicId: number): Observable<string[]> {
        return this.http.post<string[]>(this.util.baseUrl + '/api/notifications/getRetailersFollowingProduct/',{ weedId, basicId })
            .pipe(data => {
                return data;
            });
    }
}