import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { UserPosModel, Util } from '../models/';

@Injectable()
export class PosService {
    constructor(private http: HttpClient, private util: Util) { }

    getByUserAndType(userId: number, type: string) {
        return this.http.get(this.util.baseUrl + '/api/pos/getByUserAndType/' + userId + "/" + type)
            .pipe(data => {
                return data;
            });
    }

    getByUserId(userId: number) {
        return this.http.get(this.util.baseUrl + '/api/pos/getByUserId/' + userId)
            .pipe(data => {
                return data;
            });
    }

    getRetailerLocations(userId: number) {
        return this.http.get(this.util.baseUrl + '/api/list/getRetailerLocations/' + userId)
            .pipe(data => {
                return data;
            });
    }

    getWeedsByLocation(locationId: string) {
        return this.http.get(this.util.baseUrl + '/api/list/getWeedsByLocation/' + locationId)
            .pipe(data => {
                return data;
            });
    }

    removeData(userId: number, type: string) {
        return this.http.get(this.util.baseUrl + '/api/pos/removeData/' + userId + "/" + type)
            .pipe(data => {
                return data;
            });
    }

    startProcessAll() {
        return this.http.get(this.util.baseUrl + '/api/pos/startProcessAll')
            .pipe(data => {
                return data;
            });
    }

    saveCova(pos: UserPosModel) {
        var headers = new HttpHeaders({ 'Content-Type': 'application/json' });
        const body = JSON.stringify(pos);
        return this.http.post(this.util.baseUrl + '/api/pos/saveCova/', body, { headers })
            .pipe(data => {
                return data;
            });
    }

    testConnection(pos: UserPosModel) {
        var headers = new HttpHeaders({ 'Content-Type': 'application/json' });
        const body = JSON.stringify(pos);
        return this.http.post(this.util.baseUrl + '/api/pos/testConnection/', body, { headers })
            .pipe(data => {
                return data;
            });
    }

    saveGreenline(pos: UserPosModel[]) {
        var headers = new HttpHeaders({ 'Content-Type': 'application/json' });
        const body = JSON.stringify(pos);
        return this.http.post(this.util.baseUrl + '/api/pos/saveGreenline/', body, { headers })
            .pipe(data => {
                return data;
            });
    }
}
