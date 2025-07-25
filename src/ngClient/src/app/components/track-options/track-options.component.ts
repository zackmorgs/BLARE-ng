import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'track-options',
  imports: [CommonModule],
  templateUrl: './track-options.component.html',
  styleUrl: './track-options.component.scss'
})
export class TrackOptionsComponent {
  @Input() trackName: string = '';
  @Input() trackIndex: number = 0;
  @Input() trackArtist: string = '';
  @Input() trackCover: string = '';
  
  @Output() optionSelected = new EventEmitter<string>();

  onOptionClick(option: string): void {
    this.optionSelected.emit(option);
  }

  onClose(): void {
    this.optionSelected.emit('close');
  }
}
