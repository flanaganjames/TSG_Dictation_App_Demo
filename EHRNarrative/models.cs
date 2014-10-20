using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EHRNarrative
{

    public class Collection
    {
        public Dialog dialog { get; set; }
        public IEnumerable<Dialog> dialogs { get; set; }
        public IEnumerable<Group> groups { get; set; }
        public IEnumerable<Subgroup> subgroups { get; set; }
        public IEnumerable<Element> elements { get; set; }
        public IEnumerable<DialogLinkElement> dialoglinkelements { get; set; }
        public IEnumerable<TextElement> textelements { get; set; }
        public IEnumerable<ComplaintGroup> complaintgroups { get; set; }
        public Complaint complaint { get; set; }
        public IEnumerable<Group_Complaints> group_complaints { get; set; }
        public IEnumerable<Group_Complaint_Groups> group_complaint_groups { get; set; }
        public IEnumerable<Element_Complaints> element_complaints { get; set; }
        public IEnumerable<Element_Complaint_Groups> element_complaint_groups { get; set; }
        public IEnumerable<DialogElement_Complaints> dialogelement_complaints { get; set; }
        public IEnumerable<DialogElement_Complaint_Groups> dialogelement_complaint_groups { get; set; }
    }

    public class Dialog
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Next_dialog_id { get; set; }
        public bool Is_text_dialog { get; set; }
        public string Default_present_text { get; set; }
        public string Default_not_present_text { get; set; }

        public IEnumerable<Group> Groups(Collection data)
        {
            return data.groups.Where(x => x.Dialog_id == this.Id).OrderBy(x => x.Order);
        }
        public IEnumerable<Group> GroupsForComplaint(Collection data)
        {
            return this.Groups(data).Where(
                       x => x.All_complaints
                    || x.Complaints(data).Contains(data.complaint.Id)
                    || x.ComplaintGroups(data).Contains(data.complaint.Complaint_group_id));
        }
        public IEnumerable<Group> GroupsAdditional(Collection data)
        {
            return this.Groups(data).Except(this.GroupsForComplaint(data));
        }
        public IEnumerable<Element> AllElements(Collection data)
        {
            return this.Groups(data).SelectMany(x => x.AllElements(data)).OrderBy(x => x.Order);
        }
        public IEnumerable<Element> AllSelectedElements(Collection data)
        {
            return this.AllElements(data).Where(x => x.selected != null).OrderBy(x => x.Order);
        }

        public Dialog NextDialog(Collection data)
        {
            try
            {
                return data.dialogs.Where(x => x.Id == this.Next_dialog_id).First();
            }
            catch
            {
                return null;
            }
        }

    }

    public class TextDialog
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string EHR_keyword { get; set; }
        public IList<TextElement> TextElements { get; set; }
    }

    public class Group
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Dialog_id { get; set; }
        public int Order { get; set; }

        public bool Recommended { get; set; }
        public bool RecommendedActive = false;

        public List<int> Complaints(Collection data)
        {
            return data.group_complaints.Where(x => x.Group_id == this.Id).Select(x => x.Complaint_id).ToList();
        }
        public List<int> ComplaintGroups(Collection data)
        {
            return data.group_complaint_groups.Where(x => x.Group_id == this.Id).Select(x => x.Complaintgroup_id).ToList();
        }

        public Dialog Dialog(Collection data)
        {
            return data.dialogs.Where(x => x.Id == this.Dialog_id).FirstOrDefault();
        }
        public IEnumerable<Subgroup> Subgroups(Collection data)
        {
            return data.subgroups.Where(x => x.Group_id == this.Id).OrderBy(x => x.Order);
        }
        public IEnumerable<Element> Elements(Collection data)
        {
            return data.elements.Where(x => x.Group_id == this.Id).OrderBy(x => x.Order);
        }
        public IEnumerable<DialogLinkElement> DialogLinkElements(Collection data)
        {
            return data.dialoglinkelements.Where(x => x.Group_id == this.Id).OrderBy(x => x.Order);
        }
        public IEnumerable<TextElement> TextElements(Collection data)
        {
            return data.textelements.Where(x => x.Group_id == this.Id).OrderBy(x => x.Order);
        }
        public IEnumerable<Element> AllElements(Collection data)
        {
            IEnumerable<Element> elements = data.elements.Where(x => x.Group_id == this.Id);
            foreach (Subgroup subgroup in this.Subgroups(data))
            {
                elements = elements.Union(subgroup.Elements(data));
            }
            return elements.OrderBy(x => x.Order);
        }

        public bool All_complaints { get; set; }

        public IEnumerable<Element> ElementsForComplaint(Collection data)
        {
            return this.Elements(data).Where(
                       x => x.All_complaints
                    || x.Complaints(data).Contains(data.complaint.Id)
                    || x.ComplaintGroups(data).Contains(data.complaint.Complaint_group_id));
        }
        public IEnumerable<Element> ElementsAdditional(Collection data)
        {
            return this.Elements(data).Where(x => x.Subgroup_id == 0).Except(this.ElementsForComplaint(data));
        }
        public IEnumerable<DialogLinkElement> DialogLinkElementsForComplaint(Collection data)
        {
            return this.DialogLinkElements(data).Where(
                       x => x.All_complaints
                    || x.Complaints(data).Contains(data.complaint.Id)
                    || x.ComplaintGroups(data).Contains(data.complaint.Complaint_group_id));
        }
        public IEnumerable<DialogLinkElement> DialogLinkElementsAdditional(Collection data)
        {
            return this.DialogLinkElements(data).Where(x => x.Subgroup_id == 0).Except(this.DialogLinkElementsForComplaint(data));
        }
        public int ItemCount(Collection data)
        {
            return this.ElementsForComplaint(data).Count() 
                + this.DialogLinkElementsForComplaint(data).Count() 
                + this.Subgroups(data).Count() 
                + (this.ElementsAdditional(data).Any() || this.DialogLinkElementsAdditional(data).Any() ? 1 : 0);
        }
        public int SelectedItemCount(Collection data)
        {
            return this.AllElements(data).Where(x => x.selected != null).Count();
        }
        public void SetAllDefaults(Collection data)
        {
            foreach (Element element in this.ElementsForComplaint(data))
            {
                if (element.Default_present)
                    element.selected = "present";
            }
        }
        public void SetAllNormal(Collection data)
        {
            foreach (Element element in this.ElementsForComplaint(data))
            {
                element.selected = element.Is_present_normal ? "present" : "not present";
            }
        }
    }
    public class Subgroup
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Group_id { get; set; }
        public int Order { get; set; }

        public IEnumerable<Element> Elements(Collection data)
        {
            return data.elements.Where(x => x.Subgroup_id == this.Id).OrderBy(x => x.Order);
        }

        public IEnumerable<DialogLinkElement> DialogLinkElements(Collection data)
        {
            return data.dialoglinkelements.Where(x => x.Group_id == this.Id).OrderBy(x => x.Order);
        }
    }

    public class TextElement
    {
        public int Id { get; set; }
        public int Group_id { get; set; }
        public int Order { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string EHR_keyword { get; set; }

        public bool selected { get; set; }
    }

    public class Element
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Order { get; set; }

        public Dialog Dialog { get; set; }

        public int Group_id { get; set; }
        public int Subgroup_id { get; set; }
        public List<int> Complaints(Collection data)
        {
            return data.element_complaints.Where(x => x.Element_id == this.Id).Select(x => x.Complaint_id).ToList();
        }
        public List<int> ComplaintGroups(Collection data)
        {
            return data.element_complaint_groups.Where(x => x.Element_id == this.Id).Select(x => x.Complaintgroup_id).ToList();
        }

        public bool Recommended { get; set; }
        public bool RecommendedActive = false;
        public bool All_complaints { get; set; }
        public string EHR_keyword { get; set; }
        public string EHR_replace { get; set; }
        public string SLC_command { get; set; }
        public bool Is_present_normal { get; set; }
        public bool Default_present { get; set; }
        private string _Present_text;
        public string Present_text
        {
            get
            {
                if (this._Present_text == null || this._Present_text == "")
                    return this.Dialog.Default_present_text != null ? this.Dialog.Default_present_text + " " + this.Name : this.Name;
                else
                    return this._Present_text;
            }
            set
            {
                this._Present_text = value;
            }
        }
        private string _Not_present_text { get; set; }
        public string Not_present_text
        {
            get
            {
                if (this._Not_present_text == null || this._Not_present_text == "")
                    return this.Dialog.Default_not_present_text != null ? this.Dialog.Default_not_present_text + " " + this.Name : "negative " + this.Name;
                else
                    return this._Not_present_text;
            }
            set
            {
                this._Not_present_text = value;
            }
        }

        public string Display(Collection data)
        {
            return this.Name + " " + this.Subgroup_id + "|" + this.All_complaints.ToString() + "|" + String.Join(", ", this.Complaints(data)) + "|" + String.Join(", ", this.ComplaintGroups(data));
        }

        // Non-peristant
        public string selected { get; set; }
        public string normal
        {
            get
            {
                if (this.selected == "present" && this.Is_present_normal)
                    return "normal";
                else if (this.selected == "present" && !this.Is_present_normal)
                    return "abnormal";
                else if (this.selected == "not present" && this.Is_present_normal)
                    return "abnormal";
                else if (this.selected == "not present" && !this.Is_present_normal)
                    return "normal";
                else
                    return null;
            }
        }
        public string EHRString
        {
            get
            {
                if (this.selected == "present")
                    return this.Present_text.Trim();
                else if (this.selected == "not present")
                    return this.Not_present_text.Trim();
                else
                    return "";
            }
        }
    }

    public class DialogLinkElement
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Order { get; set; }

        public int Group_id { get; set; }
        public int Subgroup_id { get; set; }
        public List<int> Complaints(Collection data)
        {
            return data.element_complaints.Where(x => x.Element_id == this.Id).Select(x => x.Complaint_id).ToList();
        }
        public List<int> ComplaintGroups(Collection data)
        {
            return data.element_complaint_groups.Where(x => x.Element_id == this.Id).Select(x => x.Complaintgroup_id).ToList();
        }

        public bool All_complaints { get; set; }
        public int Linked_dialog_id { get; set; }
    }

    public class ComplaintGroup
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public IList<Complaint> Complaint { get; set; }
    }
    public class Complaint
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Complaint_group_id { get; set; }
    }

    public class Group_Complaints
    {
        public int Group_id { get; set; }
        public int Complaint_id { get; set; }
    }
    public class Group_Complaint_Groups
    {
        public int Group_id { get; set; }
        public int Complaintgroup_id { get; set; }
    }
    public class Element_Complaints
    {
        public int Element_id { get; set; }
        public int Complaint_id { get; set; }
    }
    public class Element_Complaint_Groups
    {
        public int Element_id { get; set; }
        public int Complaintgroup_id { get; set; }
    }
    public class DialogElement_Complaints
    {
        public int Dialogelement_id { get; set; }
        public int Complaint_id { get; set; }
    }
    public class DialogElement_Complaint_Groups
    {
        public int Dialogelement_id { get; set; }
        public int Complaintgroup_id { get; set; }
    }

}
