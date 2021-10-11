import { Directive, ElementRef, HostListener } from '@angular/core';

@Directive({
  selector: '[jumpToFirstInvalid]'
})
export class JumpToFirstInvalidDirective {

  constructor(private el: ElementRef) {
  }

  @HostListener('click', ['$event'])
  onClick(): void {
    setTimeout(() => {
      let element = this.el.nativeElement.parentElement;
      while (element != null && element.parentElement && element.parentElement != null) {
        element = element.parentElement;
      }
      if (element != null) {
        let firstInvalidControl = element.getElementsByClassName(
          "errorFix"
        );
        if (firstInvalidControl && firstInvalidControl.length > 0) {
          firstInvalidControl[0].scrollIntoView({ behavior: 'smooth', block: 'center' });
        }
      }
    }, 20);

  }

}
