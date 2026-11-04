CRUD de Empleados con SQLite y ListView
Desarrolle una aplicación en .NET MAUI (C# y XAML) que administre la tabla Empleados en una base de datos local SQLite, mostrando los registros en un ListView.

Estructura mínima de la tabla Empleados:

| Campo        	| Tipo     	| Observación                    	|
|--------------	|----------	|--------------------------------	|
| Id           	| int      	| Llave primaria autoincremental 	|
| Nombres      	| string   	| Obligatorio                    	|
| Apellidos    	| string   	| Obligatorio                    	|
| Puesto       	| string   	| Obligatorio                    	|
| Salario      	| decimal  	| Mayor que cero                 	|
| Correo       	| string   	| Formato de correo válido       	|
| FechaIngreso 	| DateTime 	| Seleccionada con DatePicker    	|

Requisitos:

- Crear la base de datos y la tabla Empleados al iniciar la aplicación.
- Registrar empleados desde un formulario con validación de datos.
- Mostrar todos los empleados en un ListView con una plantilla personalizada.
- Seleccionar un empleado de la lista para editar sus datos y guardar los cambios.
- Eliminar un empleado con confirmación previa.

Rúbrica del Proyecto:

| Criterio                         	| Descripción del desempeño esperado                                                                                                                                                           	|
|----------------------------------	|----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------	|
| 1. Modelo y base de datos SQLite 	| Clase Empleado con todos los campos y atributos PrimaryKey y AutoIncrement; servicio de base de datos con SQLiteAsyncConnection, creación de la tabla y ruta en FileSystem.AppDataDirectory. 	|
| 2. Insertar con validación       	| Formulario que registra empleados y valida campos obligatorios, correo y salario numérico mayor que cero, con mensajes al usuario.                                                           	|
| 3. Listado en ListView           	| ListView con ItemsSource e ItemTemplate que muestra nombre completo, puesto y salario; la lista se refresca después de cada operación.                                                       	|
| 4. Actualizar                    	| Al seleccionar un empleado en el ListView se cargan sus datos en el formulario y los cambios se guardan con UpdateAsync.                                                                     	|
| 5. Eliminar                      	| Elimina el empleado seleccionado con DeleteAsync, previa confirmación con DisplayAlert, y actualiza la lista.                                                                                	|