var createChatBtn = document.getElementById('create-chat-btn')
var createChatModal = document.getElementById('create-chat-modal')

createChatBtn.addEventListener('click', function () {
    createChatModal.classList.add('active')
})

function closeModal() {
    createChatModal.classList.remove('active')
}