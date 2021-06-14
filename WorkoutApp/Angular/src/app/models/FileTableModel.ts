export class FileTableModel {
  public position: number;
  public file: File;
  public preview: string | ArrayBuffer;

  constructor(position: number, file: File, preview: string | ArrayBuffer) {
    this.position = position;
    this.file = file;
    this.preview = preview;
  }
}
