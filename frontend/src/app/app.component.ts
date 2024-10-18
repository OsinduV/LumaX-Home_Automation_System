import { Component, OnInit } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { SignalrService } from './services/signalr.service';
import { CommonModule } from '@angular/common';
import { UserComponent } from './user/user.component';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, CommonModule, UserComponent],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent implements OnInit {
  
  // constructor(public signalrService: SignalrService){}

  ngOnInit(): void {
    // this.signalrService.startConnection();
    // this.signalrService.addTemperatureListener();
  }

}
