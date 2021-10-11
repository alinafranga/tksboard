import { Injectable } from "@angular/core";
import { Observable, Subject } from "rxjs";

@Injectable()
export class FollowListMessageService {
    private triggerComponentAction = new Subject<boolean>();
    constructor() { }

    public getTrigger(): Observable<boolean> {
        return this.triggerComponentAction.asObservable();
    }

    public updateTrigger(trigger: boolean): void {
        this.triggerComponentAction.next(trigger);
    }
}