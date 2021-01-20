import { BrowserModule } from "@angular/platform-browser";
import { NgModule } from "@angular/core";
import { HttpClientModule } from "@angular/common/http";
import { FormsModule } from "@angular/forms";
import { TooltipModule, BsDropdownModule, ModalModule } from 'ngx-bootstrap'
import { AppRoutingModule } from "./app-routing.module";

import { AppComponent } from "./app.component";
import { EventosComponent } from "./eventos/eventos.component";
import { NavComponent } from './nav/nav.component';

import { DateTimeFormatPipePipe } from "./_helps/DateTimeFormatPipe.pipe";

import { EventoService } from "./_services/evento.service";

@NgModule({
  declarations: [AppComponent,
                 EventosComponent,
                 NavComponent,
                 DateTimeFormatPipePipe],
  imports: [BrowserModule,
            AppRoutingModule,
            HttpClientModule,
            FormsModule,
            BsDropdownModule.forRoot(),
            TooltipModule.forRoot(),
            ModalModule.forRoot()
          ],
  providers: [EventoService],
  bootstrap: [AppComponent],
})
export class AppModule {}
