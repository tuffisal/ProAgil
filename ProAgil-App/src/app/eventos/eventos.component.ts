import { Component, OnInit, TemplateRef } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { BsModalRef, BsModalService } from 'ngx-bootstrap';
import { Evento } from '../_models/Evento';
import { EventoService } from '../_services/evento.service';
// Imports para o datepicker do ngx
import { defineLocale, BsLocaleService, ptBrLocale } from 'ngx-bootstrap';
import { ToastrService } from 'ngx-toastr';

defineLocale('pt-br', ptBrLocale);

@Component({
  selector: 'app-eventos',
  templateUrl: './eventos.component.html',
  styleUrls: ['./eventos.component.css']
})
export class EventosComponent implements OnInit {

  titulo = 'Eventos';

  eventosFiltrados: Evento[] = [];
  eventos: Evento[] = [];
  evento: Evento;
  imagemLargura = 50;
  imagemMargem = 2;
  mostrarImagem = false;
  registerForm: FormGroup;
  modoSalvar = 'post';
  bodyDeletarEvento = '';
  dataAtual: string;

  file: File;

  _filtroLista = '';
  fileNameToUpdate: string;

  constructor(
    private eventoService: EventoService,
    private modalService: BsModalService,
    private fb: FormBuilder,
    private localeService: BsLocaleService,
    private toastr: ToastrService
  ) {
    this.localeService.use('pt-br');
  }

  get filtroLista(): string {
    return this._filtroLista;
  }

  set filtroLista(value: string) {
    this._filtroLista = value;
    this.eventosFiltrados = this.filtroLista ? this.filtrarEventos(this.filtroLista) :
                                               this.eventos;
  }

  editarEvento(evento: Evento, template: any) {
    this.modoSalvar = 'put';
    this.openModal(template);
    // Passo a limpar a cópia do objeto passado como parâmetro
    // Isso evita alterações no grid antes de gravar as informações do modal de edição
    // Neste caso, a imagem
    this.evento = Object.assign({}, evento);
    this.fileNameToUpdate = evento.imagemURL.toString();
    this.evento.imagemURL = '';
    this.registerForm.patchValue(this.evento);
  }

  novoEvento(template: any) {
    this.modoSalvar = 'post';
    this.openModal(template);
  }

  excluirEvento(evento: Evento, template: any) {
    this.openModal(template);
    this.evento = evento;
    this.bodyDeletarEvento = `Tem certeza que deseja excluir o evento [${evento.tema}] com o código [${evento.id}]?`;
  }

  confirmDelete(template: any) {
    this.eventoService.deleteEvento(this.evento.id).subscribe(
      () => {
        template.hide();
        this.getEventos();
        this.toastr.success('Deletado com sucesso!');
      }, error => {
        this.toastr.error('Erro ao tentar deletar!');
        console.log(error);
      }
    );
  }

  openModal(template: any) {
    this.registerForm.reset();
    template.show();
  }

  // Será executado antes do html ficar pronto
  ngOnInit() {
    this.validation();
    this.getEventos();
  }

  filtrarEventos(filtrarPor: string): Evento[] {
    filtrarPor = filtrarPor.toLocaleLowerCase();
    return this.eventos.filter(
      evento => evento.tema.toLocaleLowerCase().indexOf(filtrarPor) !== -1
    );
  }

  alternarImagem() {
    this.mostrarImagem = !this.mostrarImagem;
  }

  validation() {
    this.registerForm = this.fb.group({
      tema: ['', [Validators.required, Validators.minLength(4), Validators.maxLength(50)]],
      local: ['', Validators.required],
      dataEvento: ['', Validators.required],
      qtdPessoas: ['', [Validators.required, Validators.max(120000)]],
      imagemURL: ['', Validators.required],
      telefone: ['', Validators.required],
      email: ['', [Validators.required, Validators.email]],
    });
  }

  uploadImagem() {
    if (this.modoSalvar === 'post') {
        const nomeArquivo = this.evento.imagemURL.split('\\', 3);
        this.evento.imagemURL = nomeArquivo[2];
        this.eventoService.postUpload(this.file, nomeArquivo[2]).subscribe(
          () => {
            // Esse dataAtual é uma gambiarra (foi adicionar na URL da imagem no html também)
            // utilizada para "enganar" o browser, informando que a página foi alterada (evita que ele busque do cache)
            // Isso precisou ser feito porque o nome da imagem é mantido. Logo, o nome da imagem é o mesmo mas o conteúdo
            // dela é diferente.
            this.dataAtual = new Date().getMilliseconds().toString();
            this.getEventos();
          }
        );
    } else {
        this.evento.imagemURL = this.fileNameToUpdate;
        this.eventoService.postUpload(this.file, this.fileNameToUpdate).subscribe(
          () => {
            this.dataAtual = new Date().getMilliseconds().toString();
            console.log(this.dataAtual);
            this.getEventos();
          }
        );
    }
}

  salvarAlteracao(template: any) {
    if (this.registerForm.valid) {
      if (this.modoSalvar === 'post') {
        // Copiando o objeto
        this.evento = Object.assign({}, this.registerForm.value);

        this.uploadImagem();

        this.eventoService.postEvento(this.evento).subscribe(
          (novoEvento: Evento) => {
            template.hide();
            this.getEventos();
            this.toastr.success('Inserido com sucesso!');
          }, error => {
            this.toastr.error('Erro ao tentar inserir!');
            console.log(error);
          }
        );
      } else {
        // Copiando o objeto
        // Neste caso utilizo o id: this.evento.id pq no this.registerForm.value não tem esse valor (na edição não aparece o id)
        this.evento = Object.assign({id: this.evento.id}, this.registerForm.value);

        this.uploadImagem();

        this.eventoService.putEvento(this.evento).subscribe(
          (novoEvento: Evento) => {
            template.hide();
            this.getEventos();
            this.toastr.success('Atualizado com sucesso!');
          }, error => {
            this.toastr.error('Erro ao tentar atualizar!');
            console.log(error);
          }
        );
      }
    }
  }

  onFileChange(event) {
    const reader = new FileReader();

    if (event.target.files && event.target.files.length) {
      this.file = event.target.files;
    }
  }

  getEventos() {
    this.eventoService.getAllEventos().subscribe(
      (_eventos: Evento[]) => {
      this.eventos = _eventos;
      this.eventosFiltrados = _eventos;
    }, error => {
      this.toastr.error(`Erro ao tentar carregar os eventos: ${error}`);
    });
  }

}
