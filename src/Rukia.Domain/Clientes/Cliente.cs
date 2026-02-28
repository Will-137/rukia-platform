using System;

namespace Rukia.Domain.Clientes
{
    public class Cliente
    {
        public Guid Id { get; private set; }

        public string Nome { get; private set; } = default!;
        public DocumentoFiscal? Documento { get; private set; }
        public string? Email { get; private set; }
        public string? Telefone { get; private set; }

        public bool Ativo { get; private set; } = true;

        public DateTime CreatedAtUtc { get; private set; }
        public DateTime UpdatedAtUtc { get; private set; }

        private Cliente() { } // EF

        public Cliente(
            Guid id,
            string nome,
            string? documento = null,
            string? email = null,
            string? telefone = null,
            bool ativo = true,
            DateTime? createdAtUtc = null,
            DateTime? updatedAtUtc = null)
        {
            Id = id == Guid.Empty ? Guid.NewGuid() : id;

            SetNome(nome);
            SetDocumento(documento);
            SetEmail(email);
            SetTelefone(telefone);

            Ativo = ativo;

            var now = DateTime.UtcNow;
            CreatedAtUtc = createdAtUtc ?? now;
            UpdatedAtUtc = updatedAtUtc ?? now;
        }

        public void AtualizarDados(string nome, string? documento, string? email, string? telefone)
        {
            SetNome(nome);
            SetDocumento(documento);
            SetEmail(email);
            SetTelefone(telefone);

            Touch();
        }

        public void Ativar()
        {
            if (!Ativo)
            {
                Ativo = true;
                Touch();
            }
        }

        public void Desativar()
        {
            if (Ativo)
            {
                Ativo = false;
                Touch();
            }
        }

        private void Touch() => UpdatedAtUtc = DateTime.UtcNow;

        private void SetNome(string nome)
        {
            nome = (nome ?? string.Empty).Trim();

            if (nome.Length < 3 || nome.Length > 200)
                throw new ArgumentException("Nome deve ter 3..200 caracteres.", nameof(nome));

            Nome = nome;
        }

        private void SetDocumento(string? documento)
        {
            Documento = DocumentoFiscal.FromNullable(documento);
        }

        private void SetEmail(string? email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                Email = null;
                return;
            }

            email = email.Trim();

            if (email.Length < 5 || email.Length > 254)
                throw new ArgumentException("Email deve ter 5..254 caracteres.", nameof(email));

            Email = email;
        }

        private void SetTelefone(string? telefone)
        {
            if (string.IsNullOrWhiteSpace(telefone))
            {
                Telefone = null;
                return;
            }

            telefone = telefone.Trim();

            if (telefone.Length > 30)
                throw new ArgumentException("Telefone deve ter no máximo 30 caracteres.", nameof(telefone));

            Telefone = telefone;
        }
    }
}