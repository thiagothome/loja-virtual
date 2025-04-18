document.addEventListener('DOMContentLoaded', function() {
    const carousel = document.querySelector('.carousel');
    const images = carousel.querySelectorAll('img');
    const totalImages = images.length;
    
    // Ajusta a largura do carousel dinamicamente
    carousel.style.width = `${totalImages * 100}%`;
    
    // Ajusta a largura de cada imagem
    images.forEach(img => {
        img.style.width = `${100 / totalImages}%`;
    });
    
    // Cria a animação dinamicamente
    const style = document.createElement('style');
    let keyframes = `@keyframes slide { 0% { transform: translateX(0%); } `;
    
    for (let i = 0; i < totalImages; i++) {
        const startPercent = (i * 100 / totalImages);
        const endPercent = ((i + 1) * 100 / totalImages);
        keyframes += `${startPercent + 5}% { transform: translateX(-${startPercent}%); } `;
        keyframes += `${endPercent - 5}% { transform: translateX(-${startPercent}%); } `;
    }
    
    keyframes += `100% { transform: translateX(-${(totalImages - 1) * 100 / totalImages}%); } }`;
    style.innerHTML = keyframes;
    document.head.appendChild(style);
    
    // Aplica a animação
    carousel.style.animation = `slide ${totalImages * 4}s infinite`;
});