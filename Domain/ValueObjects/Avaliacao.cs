using FluentValidation;
using FluentValidation.Results;

namespace Restaurante.Domain.ValueObjects
{
    public class Avaliacao : AbstractValidator<Avaliacao>
    {
        public Avaliacao(int estrelas, string comentario)
        {
            Estrelas = estrelas;
            Comentario = comentario;

        }

        public int Estrelas { get; set; }
        public string Comentario { get; set; }
        public ValidationResult ValidationResult { get; set; }

        public virtual bool Validar()
        {
            ValidarEstrelas();
            ValidarComentario();

            ValidationResult = Validate(this);
            return ValidationResult.IsValid;
        }

        public void ValidarEstrelas()
        {
            RuleFor(c => c.Estrelas)
                .LessThanOrEqualTo(5).WithMessage("Numero de estrelas deve ser menor ou igual a 5");
        }
        public void ValidarComentario()
        {
            RuleFor(c => c.Comentario)
                .MaximumLength(100).WithMessage("Comentario deve ter no maximo 100 caracteres")
                .NotEmpty().WithMessage("Comentario não pode ser vazio");
        }
    }
}
