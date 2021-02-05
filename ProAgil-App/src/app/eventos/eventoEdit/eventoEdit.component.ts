import { Component, OnInit } from '@angular/core';
import { FormArray, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { BsLocaleService } from 'ngx-bootstrap';
import { ToastrService } from 'ngx-toastr';
import { Evento } from 'src/app/_models/Evento';
import { EventoService } from 'src/app/_services/evento.service';

@Component({
  selector: 'app-evento-edit',
  templateUrl: './eventoEdit.component.html',
  styleUrls: ['./eventoEdit.component.css']
})
export class EventoEditComponent implements OnInit {

  titulo = 'Editar evento';
  evento: Evento = new Evento(); // para isso funcionar foi preciso transformar a interface Evento em classe (arquivo evento.ts em _models)
  imagemURL = 'assets/img/upload.png';
  registerForm: FormGroup;
  file: any;
  fileNameToUpdate: string;
  dataAtual: any;

  get lotes(): FormArray {
    return <FormArray>this.registerForm.get('lotes');
  }

  get redesSociais(): FormArray {
    return <FormArray>this.registerForm.get('redesSociais');
  }

  constructor(
    private eventoService: EventoService,
    private router: ActivatedRoute,
    private fb: FormBuilder,
    private localeService: BsLocaleService,
    private toastr: ToastrService
  ) {
    this.localeService.use('pt-br');
  }
  ngOnInit() {
    this.validation();
    this.carregarEvento();
  }

  carregarEvento() {
    // o + no comando abaixo serve para converter para number
    const idEvento = +this.router.snapshot.paramMap.get('id');
    this.eventoService.getEventoById(idEvento).subscribe(
      (evento: Evento) => {
        this.evento = Object.assign({}, evento);
        this.fileNameToUpdate = evento.imagemURL.toString();
        this.dataAtual = new Date().getMilliseconds().toString();
        this.imagemURL = `http://localhost:5000/resources/images/${this.evento.imagemURL}?_ts=${this.dataAtual}`;
        this.evento.imagemURL = '';
        this.registerForm.patchValue(this.evento);

        this.evento.lotes.forEach(lote => {
          this.lotes.push(this.criaLote(lote));
        });

        this.evento.redesSociais.forEach(redeSocial => {
          this.redesSociais.push(this.criaRedeSocial(redeSocial));
        });
      }
    );
  }

  validation() {
    this.registerForm = this.fb.group({
      id: [],
      tema: ['', [Validators.required, Validators.minLength(4), Validators.maxLength(50)]],
      local: ['', Validators.required],
      dataEvento: ['', Validators.required],
      qtdPessoas: ['', [Validators.required, Validators.max(120000)]],
      imagemURL: [''],
      telefone: ['', Validators.required],
      email: ['', [Validators.required, Validators.email]],
      lotes: this.fb.array([]),
      redesSociais: this.fb.array([])
    });
  }

  criaLote(lote: any): FormGroup {
    return this.fb.group({
      id: [lote.id],
      nome: [lote.nome, Validators.required],
      quantidade: [lote.quantidade, Validators.required],
      preco: [lote.preco, Validators.required],
      dataInicio: [lote.dataInicio],
      dataFim: [lote.dataFim],
    });
  }

  criaRedeSocial(redeSocial: any): FormGroup {
    return this.fb.group({
      id: [redeSocial.id],
      nome: [redeSocial.nome, Validators.required],
      url: [redeSocial.url, Validators.required],
    });
  }

  adicionarLote() {
    this.lotes.push(this.criaLote({ id: 0 }));
  }

  adicionarRedeSocial() {
    this.redesSociais.push(this.criaRedeSocial({ id: 0 }));
  }

  removerLote(id: number) {
    this.lotes.removeAt(id);
  }

  removerRedeSocial(id: number) {
    this.redesSociais.removeAt(id);
  }

  onFileChange(files: FileList) {
    const reader = new FileReader();
    reader.onload = (event: any) => this.imagemURL = event.target.result;
    // this.file = event.target.files;
    this.file = files;
    reader.readAsDataURL(files[0]);
  }

  uploadImagem() {
    if (this.registerForm.get('imagemURL').value !== '') {
      this.eventoService.postUpload(this.file, this.fileNameToUpdate).subscribe(
        () => {
          this.dataAtual = new Date().getMilliseconds().toString();
          this.imagemURL = `http://localhost:5000/resources/images/${this.evento.imagemURL}?_ts=${this.dataAtual}`;
        }
      );
    }
  }

  salvarEvento() {
    this.evento = Object.assign({id: this.evento.id}, this.registerForm.value);
    this.evento.imagemURL = this.fileNameToUpdate;

    this.uploadImagem();

    this.eventoService.putEvento(this.evento).subscribe(
      (novoEvento: Evento) => {
        this.toastr.success('Atualizado com sucesso!');
      }, error => {
        this.toastr.error('Erro ao tentar atualizar!');
        console.log(error);
      }
    );
  }
}
