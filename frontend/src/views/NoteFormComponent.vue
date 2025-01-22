<template>
    <div class="flex justify-between items-center p-4">

        <button @click="handleLogout"
            class="ml-auto px-4 py-2 bg-red-600 text-white rounded hover:bg-red-700 focus:outline-none focus:ring focus:ring-red-200">
            Logout
        </button>
    </div>
    <div class="max-w-xl mx-auto p-6">
        <h1 class="text-3xl font-semibold text-center">Notes App</h1>
        <div class="mt-6">
            <form @submit.prevent="addNote" class="space-y-4">
                <input v-model="newNote.title" type="text" placeholder="Note title"
                    class="w-full p-3 border border-gray-300 rounded" required />
                <textarea v-model="newNote.content" placeholder="Note content"
                    class="w-full p-3 border border-gray-300 rounded" rows="4"></textarea>
                <button type="submit" class="w-full py-2 bg-blue-500 text-white rounded hover:bg-blue-600">
                    Add Note
                </button>
            </form>
        </div>
    </div>

    <div class="container mx-auto px-4">
        <div v-if="notes.length > 0" class="mt-8">
            <h2 class="text-2xl font-semibold">Your Notes</h2>
            <div class="flex items-center space-x-2">
                <input v-model="searchQuery" type="text" placeholder="Search..."
                    class="px-4 py-2 w-80 border rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500" />
                <select v-model="selectedStatus"
                    class="px-4 py-2 w-48 border rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500">
                    <option value="">Select Status</option>
                    <option value="active">Active</option>
                    <option value="inactive">Inactive</option>
                </select>
            </div>



            <div class="overflow-x-auto">
                <table class="min-w-full table-auto mt-4 border-collapse">
                    <thead>
                        <tr class="bg-gray-200">
                            <th class="px-4 py-2 text-left">Title</th>
                            <th class="px-4 py-2 text-left">Content</th>
                            <th class="px-4 py-2 text-left">Status</th>
                            <th class="px-4 py-2 text-left">Create At</th>
                            <th class="px-4 py-2 text-left">Update At</th>
                            <th class="px-4 py-2 text-left">Actions</th>
                        </tr>
                    </thead>
                    <tbody>
                        <tr v-for="(note, index) in filteredNotes" :key="note.id" class="border-b">
                            <td class="px-4 py-2">{{ note.title }}</td>
                            <td class="px-4 py-2">{{ truncateContent(note.content, 15) }}</td>
                            <td class="px-4 py-2">
                                <span :class="{
                                    'bg-green-700 text-white px-2 py-1 backdrop-blur-sm bg-opacity-50 rounded-sm': note.status === true,
                                    'bg-red-700 text-white px-2 py-1 backdrop-blur-sm bg-opacity-50 rounded-sm': note.status === false
                                }">
                                    {{ note.status ? 'Active' : 'Inactive' }}
                                </span>
                            </td>
                            <td class="px-4 py-2">{{ formatDate(note.createdAt) }}</td>
                            <td class="px-4 py-2">{{ formatDate(note.updatedAt) }}</td>
                            <td class="px-4 py-2">
                                <button @click="openReadModal(note)"
                                    class="bg-green-500 text-white hover:bg-green-600 focus:outline-none focus:ring-2 focus:ring-blue-500 focus:ring-opacity-50 py-2 px-4 rounded-lg mr-2">
                                    Read
                                </button>
                                <button @click="openEditModal(index)"
                                    class="bg-blue-500 text-white hover:bg-blue-600 focus:outline-none focus:ring-2 focus:ring-blue-500 focus:ring-opacity-50 py-2 px-4 rounded-lg mr-2">
                                    Edit
                                </button>
                                <button @click="deleteNote(index)"
                                    class="bg-red-600 text-white hover:bg-red-700 focus:outline-none focus:ring-2 focus:ring-red-500 focus:ring-opacity-50 py-2 px-4 rounded-lg">
                                    Delete
                                </button>
                            </td>
                        </tr>
                    </tbody>
                </table>
            </div>



        </div>
    </div>

    <div v-if="showModal" class="fixed inset-0 z-50 flex justify-center items-center bg-black bg-opacity-50">
        <div class="bg-white p-6 rounded-lg shadow-lg w-1/2">
            <h2 class="text-2xl font-semibold mb-4">Edit Note</h2>
            <form @submit.prevent="updateNote">
                <div class="mb-4">
                    <label for="title" class="block text-sm font-medium text-gray-700">Title</label>
                    <input v-model="oldNote.title" type="text" id="title" class="w-full p-2 border rounded-lg"
                        required />
                </div>
                <div class="mb-4">
                    <label for="content" class="block text-sm font-medium text-gray-700">Content</label>
                    <textarea v-model="oldNote.content" id="content" rows="4"
                        class="w-full p-2 border rounded-lg"></textarea>
                </div>
                <div class="mb-4">
                    <label for="status" class="block text-sm font-medium text-gray-700">Status</label>
                    <select v-model="oldNote.status" id="status" class="w-full p-2 border rounded-lg">
                        <option :value="true">Active</option>
                        <option :value="false">Inactive</option>
                    </select>
                </div>
                <div class="flex justify-end">
                    <button type="submit"
                        class="bg-blue-500 text-white hover:bg-blue-600 focus:outline-none focus:ring-2 focus:ring-blue-500 focus:ring-opacity-50 py-2 px-4 rounded-lg">
                        Save Changes
                    </button>
                    <button @click="closeModal"
                        class="ml-2 bg-gray-500 text-white hover:bg-gray-600 focus:outline-none focus:ring-2 focus:ring-gray-500 focus:ring-opacity-50 py-2 px-4 rounded-lg">
                        Cancel
                    </button>
                </div>
            </form>
        </div>
    </div>

    <div v-if="showModalRead" class="fixed inset-0 z-50 flex justify-center items-center bg-black bg-opacity-50">
        <div class="bg-white p-6 rounded-lg shadow-lg w-1/2">
            <h2 class="text-2xl font-semibold mb-4">Read Note</h2>
            <div class="mb-4">
                <h3 class="text-xl underline">{{ selectedNote.title }}</h3>
            </div>
            <div class="mb-4">
                <p class="text-sm">{{ selectedNote.content }}</p>
            </div>
            <div class="mb-4">
                <p class="text-xs">{{ formatDate(selectedNote.createdAt) }}</p>
            </div>
            <div class="flex justify-end">
                <button @click="closeModal"
                    class="ml-2 bg-gray-500 text-white hover:bg-gray-600 focus:outline-none focus:ring-2 focus:ring-gray-500 focus:ring-opacity-50 py-2 px-4 rounded-lg">
                    Cancel
                </button>
            </div>
        </div>
    </div>

</template>

<script lang="ts">
import axios from '@/axios';
import Swal from 'sweetalert2';


interface Note {
    id: number;
    title: string;
    content: string;
    createdAt: string;
    status: boolean;
    updatedAt: string;
    userId: number;
}
export default {
    data() {
        return {
            searchQuery: '',
            selectedStatus: '',
            filterText: '',
            sortKey: 'createdAt',
            sortOrder: 'asc',
            selectedNote: {} as Note,
            newNote: {
                title: '',
                content: '',
            },
            oldNote: {
                title: '',
                content: '',
            } as Note,
            notes: [] as Note[],
            showModal: false,
            editingNoteId: null as number | null,
            showModalRead: false,
            jwtToken: localStorage.getItem('jwtToken') || '',
            userId: localStorage.getItem('userId') || '',
        };
    },
    mounted() {
        this.fetchNotes();
    },
    computed: {
        filteredNotes() {
            return this.notes.filter(note => {
                const matchesSearch = note.title.toLowerCase().includes(this.searchQuery.toLowerCase()) || note.content.toLowerCase().includes(this.searchQuery.toLowerCase());
                const matchesStatus = this.selectedStatus === '' || (this.selectedStatus === 'active' && note.status) || (this.selectedStatus === 'inactive' && !note.status);
                return matchesSearch && matchesStatus;
            });
        }
    },
    methods: {

        handleLogout() {
            const router = this.$router;
            console.log('Logging out...');
            localStorage.removeItem('authToken');
            localStorage.removeItem('refreshToken');
            localStorage.removeItem('userId');
            if (router) {
                console.log('Navigating to login form...');
                router.push({ name: 'loginForm' });
            } else {
                console.error('Router is undefined');
            }
        },
        truncateContent(content: string, wordLimit: number): string {
            const words = content.split(' ');
            if (words.length > wordLimit) {
                return words.slice(0, wordLimit).join(' ') + '...';
            }
            return content;
        },
        formatDate(date: string): string {
            const formattedDate = new Date(date);
            return formattedDate.toLocaleString();
        },
        async fetchNotes() {
            try {
                if (!this.jwtToken || !this.userId) {
                    Swal.fire({
                        title: 'Error!',
                        text: 'Token or User ID is missing. Please log in again.',
                        icon: 'error',
                        confirmButtonText: 'OK',
                    });
                    return;
                }
                const response = await axios.get('/api/Notes', {
                    headers: {
                        Authorization: `Bearer ${this.jwtToken}`,
                    },
                });
                if (response.data && Array.isArray(response.data.data)) {
                    this.notes = response.data.data;
                } else {
                    console.warn('Unexpected response format:', response.data);
                    this.notes = [];
                }
            } catch (error: unknown) {
                console.error('Error fetching notes:', error);
                const errorMessage = (error as { response?: { data?: { message?: string } } }).response?.data?.message
                    || 'Failed to fetch notes. Please try again later.';
                Swal.fire({
                    title: 'Error!',
                    text: errorMessage,
                    icon: 'error',
                    confirmButtonText: 'OK',
                });
                this.notes = [];
            }
        },


        async addNote(): Promise<void> {
            if (this.newNote.title && this.newNote.content) {
                if (!this.jwtToken || !this.userId) {
                    console.error('Token or User ID is missing or invalid');
                    Swal.fire({
                        title: 'Error!',
                        text: 'You need to be logged in to perform this action.',
                        icon: 'error',
                        confirmButtonText: 'OK',
                    });
                    return;
                }
                try {
                    const newNote = {
                        Title: this.newNote.title,
                        Content: this.newNote.content,
                        UserId: this.userId,
                    };
                    await axios.post('/api/Notes', newNote, {
                        headers: {
                            'Accept': 'application/json, text/plain, */*',
                            Authorization: `Bearer ${this.jwtToken}`,
                        },
                    });
                    Swal.fire({
                        title: 'Success!',
                        text: 'Your note has been added.',
                        icon: 'success',
                        confirmButtonText: 'OK',
                    });
                    this.fetchNotes();
                    this.newNote.title = '';
                    this.newNote.content = '';
                } catch (error) {
                    console.error('There was an error adding the note:', error);
                    Swal.fire({
                        title: 'Error!',
                        text: 'There was an issue adding your note. Please try again later.',
                        icon: 'error',
                        confirmButtonText: 'OK',
                    });
                }
            }
        },
        openEditModal(index: number): void {
            const note = this.notes[index];
            if (note.userId) {
                Swal.fire({
                    title: 'Error!',
                    text: 'You cannot edit this note. This is not your note.',
                    icon: 'error',
                    confirmButtonText: 'OK',
                });
                return;
            }
            this.oldNote = { ...note };
            this.showModal = true;
            this.editingNoteId = note.id;
        },
        openReadModal(note: Note) {
            this.selectedNote = { ...note };
            this.showModalRead = true;
        },
        closeModal(): void {
            this.showModal = false;
            this.editingNoteId = null;
            this.showModalRead = false;
        },
        async updateNote(): Promise<void> {
            if (this.oldNote.title && this.oldNote.content && this.editingNoteId) {
                if (!this.jwtToken || !this.userId) {
                    console.error('Token or User ID is missing or invalid');
                    Swal.fire({
                        title: 'Error!',
                        text: 'You need to be logged in to perform this action.',
                        icon: 'error',
                        confirmButtonText: 'OK',
                    });
                    return;
                }
                if (this.oldNote.userId) {
                    Swal.fire({
                        title: 'Error!',
                        text: 'You cannot update this note. This is not your note.',
                        icon: 'error',
                        confirmButtonText: 'OK',
                    });
                    return;
                }
                try {
                    const updatedNote = {
                        Title: this.oldNote.title,
                        Content: this.oldNote.content,
                        Status: this.oldNote.status,
                    };
                    await axios.put(`/api/Notes/${this.editingNoteId}`, updatedNote, {
                        headers: {
                            Authorization: `Bearer ${this.jwtToken}`,
                        },
                    });
                    Swal.fire({
                        title: 'Updated!',
                        text: 'Your note has been updated.',
                        icon: 'success',
                        confirmButtonText: 'OK',
                    });
                    this.fetchNotes();
                    this.closeModal();
                } catch (error) {
                    console.error('There was an error updating the note:', error);
                    Swal.fire({
                        title: 'Error!',
                        text: 'There was an issue updating your note. Please try again later.',
                        icon: 'error',
                        confirmButtonText: 'OK',
                    });
                }
            }
        },
        async deleteNote(index: number): Promise<void> {
            const noteId = this.notes[index].id;
            if (this.notes[index].userId) {
                Swal.fire({
                    title: 'Error!',
                    text: 'You cannot delete this note. This is not your note.',
                    icon: 'error',
                    confirmButtonText: 'OK',
                });
                return;
            }
            Swal.fire({
                title: 'Are you sure?',
                text: 'Do you want to delete this note?',
                icon: 'warning',
                showCancelButton: true,
                confirmButtonText: 'Yes, delete it!',
                cancelButtonText: 'No, keep it',
            }).then(async (result) => {
                if (result.isConfirmed) {
                    try {
                        await axios.delete(`/api/Notes/${noteId}`, {
                            headers: {
                                Authorization: `Bearer ${this.jwtToken}`,
                            },
                        });
                        Swal.fire({
                            title: 'Deleted!',
                            text: 'Your note has been deleted.',
                            icon: 'success',
                            confirmButtonText: 'OK',
                        });
                        this.fetchNotes();
                    } catch (error) {
                        console.error('There was an error deleting the note:', error);
                        Swal.fire({
                            title: 'Error!',
                            text: 'There was an issue deleting your note. Please try again later.',
                            icon: 'error',
                            confirmButtonText: 'OK',
                        });
                    }
                }
            });
        },
    },
};
</script>

<style scoped></style>
